﻿using Extensions.Standard;
using LoggerLite;
using ProgressReporting;
using Stocks.Data.Ef;
using Stocks.Data.Model;
using Stocks.Data.TradingSimulator.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using Stocks.Data.Common.Models;

namespace Stocks.Data.TradingSimulator
{
    public class Top10TradingSimulator : TradingSimulatorBase, ITradingSimulator
    {
        private readonly IStockQuoteRepository _stockQuoteRepository;
        private readonly IProjectSettings _projectSettings;
        public Top10TradingSimulator(ILogger logger, IStockQuoteRepository stockQuoteRepository, IProjectSettings projectSettings) : base(logger)
        {
            _stockQuoteRepository = stockQuoteRepository;
            _projectSettings = projectSettings;
        }
        public int TopN { get; set; } = 10;

        public override SimulationResult Simulate(ITradingSimulationConfig tradingSimulationConfig, IProgressReportable progress = null)
        {
            var result = base.Simulate(tradingSimulationConfig, progress);

            var allQuotesPrefilterd = _stockQuoteRepository
                .GetAll(x => !_projectSettings.BlackListPattern.IsMatch(x.Ticker)
                             && x.DateParsed.InOpenRange(tradingSimulationConfig.FromDate.AddDays(-30), tradingSimulationConfig.ToDate))
                .ToList();

            var filteredQuotes = allQuotesPrefilterd.Where(x =>
                x.DateParsed.InOpenRange(tradingSimulationConfig.FromDate, tradingSimulationConfig.ToDate)).ToList();

            var tradingStartingDate = filteredQuotes.Min(x => x.DateParsed);
            var tradingEndDate = filteredQuotes.Max(x => x.DateParsed);
            var datesToTrade = filteredQuotes
                .Where(x => x.DateParsed.InOpenRange(tradingStartingDate, tradingEndDate))
                .Select(x => x.DateParsed)
                .Distinct()
                .OrderBy(x => x.Date)
                .ToList();


            progress?.Restart(datesToTrade.Count);
            foreach (var date in datesToTrade)
            {
                var topN = GetTopN(allQuotesPrefilterd, date);
                //if (topN.Count > TopN)
                //    throw new InvalidOperationException(nameof(topN));
                //if (topN.Any(x => x.DateParsed >= date))
                //    throw new InvalidOperationException(nameof(topN));

                var topNTickers = topN.Select(quote => quote.Ticker).ToHashSet();
                var tradingDayQuotesForMostRising = allQuotesPrefilterd
                    .Where(x => x.DateParsed.Date.Equals(date.Date) && topNTickers.Contains(x.Ticker) && x.IsValid());

                foreach (var stockQuoteForToday in tradingDayQuotesForMostRising)
                {
                    var volume = (Balance / TopN) / (stockQuoteForToday.Open);
                    var buyOrderStatus = PlaceBuyOrder(stockQuoteForToday, stockQuoteForToday.Open, volume);
                    if (buyOrderStatus == OrderStatusType.Accepted)
                    {
                        var sellOrderStatus = ClosePosition(stockQuoteForToday, stockQuoteForToday.Close);
                        if (sellOrderStatus != OrderStatusType.Accepted)
                        {
                            _logger.LogWarning($"Could not perform closing sell on {stockQuoteForToday.Ticker} {stockQuoteForToday.DateParsed}. Selling the trade for buy price...");
                            ClosePosition(stockQuoteForToday, stockQuoteForToday.Open);
                        }
                    }
                    result.ROC.Activate(true, stockQuoteForToday.Close > stockQuoteForToday.Open);
                }
                progress?.ReportProgress();
            }

            result.TransactionsLedger = TransactionsLedger;
            result.FinalBalance = Balance;
            result.TradingSimulationConfig = tradingSimulationConfig;

            return result;
        }

        public List<StockQuote> GetSignals(DateTime date)
        {
            var allQuotesFromLastSession = _stockQuoteRepository.GetAllQuotesFromPreviousSession(date);
            return GetTopN(allQuotesFromLastSession, date);
        }

        private List<StockQuote> GetTopN(List<StockQuote> allQuotesPrefilterd, DateTime date)
        {
            var allQuotesBeforeTradeDay = allQuotesPrefilterd.Where(x => x.DateParsed.Date < date.Date).ToList();
            var nMinusOneDay = allQuotesBeforeTradeDay.Select(x => x.DateParsed).Max();
            var allQuotesFromMinusOneDay = allQuotesPrefilterd.Where(x => x.DateParsed.Date.Equals(nMinusOneDay)).ToList();

            var topN = allQuotesFromMinusOneDay
                .OrderByDescending(x => x.AveragePriceChange)
                .Take(TopN)
                .ToList();

            return topN;
        }
    }
}
