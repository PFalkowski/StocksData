using Extensions.Standard;
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
            var datesToTrade = _stockQuoteRepository.GetTradingDates(tradingSimulationConfig.FromDate, tradingSimulationConfig.ToDate);
            progress?.Restart(datesToTrade.Count);
            foreach (var date in datesToTrade)
            {
                var topN = GetTopN(date);
                if (topN.Count > TopN)
                    throw new InvalidOperationException(nameof(topN));
                if (topN.Any(x => x.DateParsed >= date))
                    throw new InvalidOperationException(nameof(topN));
                
                var topNTickers = topN.Select(quote => quote.Ticker).ToHashSet();
                var tradingDayQuotesForMostRising = _stockQuoteRepository.GetAll(x => x.DateParsed.Date.Equals(date.Date)
                    && topNTickers.Contains(x.Ticker));

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
            return GetTopN(date);
        }

        private List<StockQuote> GetTopN(DateTime date)
        {
            var allQuotesFromLastSession = _stockQuoteRepository.GetAllQuotesFromPreviousSession(date);
            return allQuotesFromLastSession
                .OrderByDescending(x => x.AveragePriceChange)
                .Take(TopN)
                .ToList();

            //var allQuotesBeforeTradeDay = allQuotesPrefilterd.Where(x => x.DateParsed.Date < date.Date).ToList();
            //var nMinusOneDay = allQuotesBeforeTradeDay.Select(x => x.DateParsed).Max();
            //var nMinusTwoDays = allQuotesBeforeTradeDay.Where(x => x.DateParsed.Date < nMinusOneDay).Select(x => x.DateParsed)
            //    .Max();
            //var allQuotesFromMinusTwoDays = allQuotesPrefilterd.Where(x => x.DateParsed.Date.Equals(nMinusTwoDays));
            //var allQuotesFromMinusOneDay = allQuotesPrefilterd.Where(x => x.DateParsed.Date.Equals(nMinusOneDay)).ToList();

            //foreach (var quoteFromMinusTwoDays in allQuotesFromMinusTwoDays)
            //{
            //    var quoteFromMinusOneDay =
            //        allQuotesFromMinusOneDay.SingleOrDefault(x => x.Ticker.Equals(quoteFromMinusTwoDays.Ticker));

            //    if (quoteFromMinusOneDay == null)
            //    {
            //        continue;
            //    }

            //    var change = (quoteFromMinusOneDay.AveragePrice - quoteFromMinusTwoDays.AveragePrice) /
            //                 quoteFromMinusTwoDays.AveragePrice;

            //    quoteFromMinusOneDay.AveragePriceChange = change;
            //}

            //var topMostRising = allQuotesFromMinusOneDay
            //    .OrderByDescending(x => x.AveragePriceChange)
            //    .Take(TopN)
            //    .ToList();

            //return topMostRising;
        }
    }
}
