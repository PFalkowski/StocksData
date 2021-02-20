using System;
using Extensions.Standard;
using Stocks.Data.Model;
using Stocks.Data.TradingSimulator.Models;
using System.Collections.Generic;
using System.Linq;
using LoggerLite;
using ProgressReporting;
using Stocks.Data.Common.Models;
using Stocks.Data.Ef;

namespace Stocks.Data.TradingSimulator
{
    public abstract class TradingSimulatorBase : ITradingSimulator
    {
        protected readonly ILogger Logger;
        protected readonly IStockQuoteRepository StockQuoteRepository;
        protected readonly IProjectSettings ProjectSettings;

        public TradingSimulatorBase(ILogger logger,
            IStockQuoteRepository stockQuoteRepository,
            IProjectSettings projectSettings)
        {
            Logger = logger;
            StockQuoteRepository = stockQuoteRepository;
            ProjectSettings = projectSettings;
        }

        public virtual SimulationResult Simulate(List<StockQuote> allQuotesPrefilterd,
            ITradingSimulationConfig tradingSimulationConfig,
            IProgressReportable progress = null)
        {
            var ledger = new TransactionsLedger(tradingSimulationConfig.StartingCash);
            var result = new SimulationResult { TradingSimulationConfig = tradingSimulationConfig };

            var filteredQuotes = allQuotesPrefilterd.Where(x =>
                x.DateParsed.InOpenRange(tradingSimulationConfig.FromDate, tradingSimulationConfig.ToDate)).ToList();

            var tradingStartingDate = filteredQuotes.Min(x => x.DateParsed);
            var tradingEndDate = filteredQuotes.Max(x => x.DateParsed);
            var datesToTrade = filteredQuotes
                .Where(x => x.DateParsed.InOpenRange(tradingStartingDate, tradingEndDate))
                .Select(x => x.DateParsed)
                .Distinct()
                .OrderBy(x => x)
                .ToList();


            progress?.Restart(datesToTrade.Count);
            foreach (var date in datesToTrade)
            {
                var topN = GetTopN(tradingSimulationConfig, allQuotesPrefilterd, date);

                var topNTickers = topN.Select(quote => quote.Ticker).ToHashSet();
                var tradingDayQuotesForMostRising = allQuotesPrefilterd
                    .Where(x => x.DateParsed.Date.Equals(date.Date) && topNTickers.Contains(x.Ticker) && x.IsValid());

                foreach (var stockQuoteForToday in tradingDayQuotesForMostRising)
                {
                    var volume = (ledger.Balance / tradingSimulationConfig.TopN) / (stockQuoteForToday.Open);
                    var buyOrderStatus = ledger.PlaceBuyOrder(stockQuoteForToday, stockQuoteForToday.Open, volume);

                    switch (buyOrderStatus)
                    {
                        case OrderStatusType.Accepted:
                            var sellOrderStatus = ledger.ClosePosition(stockQuoteForToday, stockQuoteForToday.Close);
                            if (sellOrderStatus != OrderStatusType.Accepted)
                            {
                                Logger.LogWarning($"Could not perform closing sell on {stockQuoteForToday.Ticker} {stockQuoteForToday.DateParsed}. Selling the trade for buy price...");
                                ledger.ClosePosition(stockQuoteForToday, stockQuoteForToday.Open);
                            }

                            switch (sellOrderStatus)
                            {
                                case OrderStatusType.DeniedNoOpenPosition:
                                    Logger.LogError($"Sell order of {volume} stocks denied due to lack of open position on {stockQuoteForToday.Ticker}.");
                                    break;
                                case OrderStatusType.DeniedOutOfRange:
                                    Logger.LogWarning($"Sell order for of {volume} stocks denied due to price being out of range of today's {stockQuoteForToday.Ticker} prices ({stockQuoteForToday.Low} - {stockQuoteForToday.High})." +
                                                      "Selling the trade for buy price...");
                                    ledger.ClosePosition(stockQuoteForToday, stockQuoteForToday.Open);
                                    break;
                            }
                            break;
                        case OrderStatusType.DeniedInsufficientFunds:
                            Logger.LogWarning($"Buy order for {stockQuoteForToday.Ticker} price = {stockQuoteForToday.Open} vol = {volume} denied due to insufficient funds ({ledger.Balance}).");
                            break;
                        case OrderStatusType.DeniedOutOfRange:
                            Logger.LogWarning($"Buy order for {stockQuoteForToday.Open * volume} denied due to price being out of range of today's {stockQuoteForToday.Ticker} prices ({stockQuoteForToday.Low} - {stockQuoteForToday.High}.");
                            break;

                    }
                    result.ROC.Activate(true, stockQuoteForToday.Close > stockQuoteForToday.Open);
                }
                progress?.ReportProgress();
            }

            result.TransactionsLedger = ledger.TheLedger;
            result.FinalBalance = ledger.Balance;

            return result;
        }

        public virtual List<StockQuote> GetSignals(ITradingSimulationConfig tradingSimulationConfig, DateTime date)
        {
            var allQuotesFromLastSession = StockQuoteRepository.GetAllQuotesFromPreviousSession(date);
            return GetTopN(tradingSimulationConfig, allQuotesFromLastSession, date);
        }

        protected abstract List<StockQuote> GetTopN(ITradingSimulationConfig tradingSimulationConfig, List<StockQuote> allQuotesPrefilterd, DateTime date);
    }
}
