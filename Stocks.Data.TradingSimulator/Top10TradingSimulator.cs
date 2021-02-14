using Extensions.Standard;
using Stocks.Data.Model;
using Stocks.Data.TradingSimulator.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using LoggerLite;

namespace Stocks.Data.TradingSimulator
{
    public class Top10TradingSimulator : TradingSimulatorBase, ITradingSimulator
    {
        private readonly ILogger _logger;
        public Top10TradingSimulator(ILogger logger)
        {
            _logger = logger;
        }
        public int TopN { get; set; } = 10;

        public (List<StockTransaction> transactionsLedger, double finalBalance) Simulate(List<Company> companies, TradingSimulationConfig tradingSimulationConfig)
        {
            Balance = tradingSimulationConfig.StartingCash;
            var regex = new Regex(tradingSimulationConfig.BlackListPattern);
            companies = companies.Where(x => !regex.IsMatch(x.Ticker)).ToList();
            var allQuotesPrefilterd = companies.SelectMany(x => x.Quotes).Where(x =>
                x.DateParsed.InOpenRange(tradingSimulationConfig.FromDate.AddDays(-30), tradingSimulationConfig.ToDate))
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

            foreach (var date in datesToTrade)
            {
                var allQuotesBeforeTradeDay = allQuotesPrefilterd.Where(x => x.DateParsed.Date < date.Date).ToList();
                var nMinusOneDay = allQuotesBeforeTradeDay.Select(x => x.DateParsed).Max();
                var nMinusTwoDays = allQuotesBeforeTradeDay.Where(x => x.DateParsed.Date < nMinusOneDay).Select(x => x.DateParsed).Max();
                var allQuotesFromMinusTwoDays = allQuotesPrefilterd.Where(x => x.DateParsed.Date.Equals(nMinusTwoDays));
                var allQuotesFromMinusOneDay = allQuotesPrefilterd.Where(x => x.DateParsed.Date.Equals(nMinusOneDay)).ToList();

                foreach (var quoteFromMinusTwoDays in allQuotesFromMinusTwoDays)
                {
                    var newerQuote =
                        allQuotesFromMinusOneDay.SingleOrDefault(x => x.Ticker.Equals(quoteFromMinusTwoDays.Ticker));

                    if (newerQuote == null)
                    {
                        continue;
                    }
                    var change = (newerQuote.AveragePrice - quoteFromMinusTwoDays.AveragePrice) /
                                 quoteFromMinusTwoDays.AveragePrice;

                    newerQuote.AveragePriceChange = change;
                }

                var topMostRising = allQuotesFromMinusOneDay.OrderByDescending(x => x.AveragePriceChange).Take(TopN).ToHashSet();
                var tradingDayQuotesForMostRising = allQuotesPrefilterd.Where(x => x.DateParsed.Date.Equals(date.Date)
                    && topMostRising.Select(x => x.Ticker).ToHashSet().Contains(x.Ticker)
                    && x.IsValid());

                foreach (var stockQuoteForToday in tradingDayQuotesForMostRising)
                {
                    var volume = (Balance / TopN) / (stockQuoteForToday.Open);
                    var bought = PlaceBuyOrder(stockQuoteForToday, stockQuoteForToday.Open, volume);
                    if (bought)
                    {
                        var sold = ClosePosition(stockQuoteForToday, stockQuoteForToday.Close);
                        if (!sold)
                        {
                            _logger.LogWarning($"Could not perform closing sell on {stockQuoteForToday.Ticker} {stockQuoteForToday.DateParsed}. Rolling back the trade...");
                            sold = ClosePosition(stockQuoteForToday, stockQuoteForToday.Open);
                        }
                    }
                }
            }

            return (TransactionsLedger, Balance);
        }
    }
}
