using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Transactions;
using LoggerLite;
using Services.IO;
using Stocks.Data.Common.Models;
using Stocks.Data.Ef;
using Stocks.Data.Ef.Repositories;
using Stocks.Data.Model;
using Stocks.Data.Services.Tier0;

namespace Stocks.Data.Services.Tier1
{
    public class StockUpdateService : IStockUpdateService
    {
        private readonly IStockQuoteRepository _stockQuoteRepository;
        private readonly ICompanyRepository _companyRepository;
        private readonly ILogger _logger;
        private readonly IDownloader _downloader;
        private readonly IProjectSettings _projectSettings;
        private readonly IStockQuotesDownloadService _stockQuotesDownloadService;
        private readonly IStocksBulkDeserializer _stocksBulkDeserializer;

        public StockUpdateService(IStockQuoteRepository stockQuoteRepository,
            ILogger logger,
            IDownloader downloader,
            IProjectSettings projectSettings,
            IStockQuotesDownloadService stockQuotesDownloadService,
            IStocksBulkDeserializer stocksBulkDeserializer,
            ICompanyRepository companyRepository)
        {
            _stockQuoteRepository = stockQuoteRepository;
            _logger = logger;
            _downloader = downloader;
            _projectSettings = projectSettings;
            _stockQuotesDownloadService = stockQuotesDownloadService;
            _stocksBulkDeserializer = stocksBulkDeserializer;
            _companyRepository = companyRepository;
        }

        public async Task PerformUpdateTillToday()
        {
            var latestDateInDb = await _stockQuoteRepository.GetLatestSessionInDbDateAsync();
            var currentDate = latestDateInDb.AddDays(1);
            var endDate = DateTime.Today;
            var datesToCheck = new List<DateTime>();
            while (currentDate.Date <= endDate.Date)
            {
                if (currentDate.DayOfWeek != DayOfWeek.Saturday && currentDate.DayOfWeek != DayOfWeek.Sunday)
                {
                    datesToCheck.Add(currentDate.Date);
                }
                currentDate = currentDate.AddDays(1);
            }

            using var scope = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled);
            //using var scope = new TransactionScope(TransactionScopeOption.Required,
            //    new TransactionOptions { IsolationLevel = IsolationLevel.ReadCommitted });

            foreach (var date in datesToCheck)
            {
                await PerformDayUpdate(date);
            }

            _stockQuoteRepository.SaveChanges();
            scope.Complete();
        }

        private async Task PerformDayUpdate(DateTime date)
        {
            string updateData;
            try
            {
                updateData = await _stockQuotesDownloadService.DownloadUpdate(_projectSettings, date);
            }
            catch (Exception e)
            {
                _logger.LogError($"Could not get update file for day {date.ToShortDateString()}. Skipping. {e.Message}");
                return;
            }
            var quotes = _stocksBulkDeserializer
                .Deserialize(updateData)
                .Where(x => !_projectSettings.ExcludeBlacklisted || !_projectSettings.BlackListPattern.IsMatch(x.Ticker))
                .ToList();

            EnrichQuotes(quotes, date);

            _stockQuoteRepository.AddRange(quotes);
        }

        private void EnrichQuotes(List<StockQuote> quotes, DateTime date)
        {
            var allQuotesFromPreviousSession = _stockQuoteRepository.GetAllQuotesFromPreviousSession(date).ToHashSet();
            foreach (var quote in quotes)
            {
                quote.Company = _companyRepository.GetById(quote.Ticker);
                if (quote.Company == null)
                {
                    var newCompanyEntity = new Company { Ticker = quote.Ticker };
                    quote.Company = newCompanyEntity;
                    _companyRepository.Add(newCompanyEntity);

                    _companyRepository.SaveChanges();
                }
                else
                {
                    quote.PreviousStockQuote = allQuotesFromPreviousSession.SingleOrDefault(x => x.Ticker.Equals(quote.Ticker));
                    if (quote.PreviousStockQuote != null)
                    {
                        quote.AveragePriceChange = (quote.AveragePrice - quote.PreviousStockQuote.AveragePrice) /
                                                quote.PreviousStockQuote.AveragePrice;
                    }
                    else
                    {
                        _logger.LogWarning($"Inconsistent state. {quote.Ticker} is present in database, yet it does not have any record from previous session dated in {allQuotesFromPreviousSession.First().DateParsed.ToShortDateString()}" +
                                           $"Add it to blacklist or data fix manually.");
                    }
                }
                quote.DateParsed = DateTime.ParseExact(quote.Date.ToString(), "yyyyMMdd",
                    CultureInfo.InvariantCulture);
            }
        }
    }
}
