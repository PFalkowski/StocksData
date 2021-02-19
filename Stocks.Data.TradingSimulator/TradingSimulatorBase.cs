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
        public double Balance { get; protected set; }
        public readonly Dictionary<string, OpenedPosition> OpenedPositions = new Dictionary<string, OpenedPosition>();
        public readonly List<StockTransaction> TransactionsLedger = new List<StockTransaction>();
        protected readonly ILogger _logger;
        protected readonly IStockQuoteRepository _stockQuoteRepository;
        protected readonly IProjectSettings _projectSettings;

        public TradingSimulatorBase(ILogger logger, IStockQuoteRepository stockQuoteRepository, IProjectSettings projectSettings)
        {
            _logger = logger;
            _stockQuoteRepository = stockQuoteRepository;
            _projectSettings = projectSettings;
        }

        public virtual SimulationResult Simulate(List<StockQuote> allQuotesPrefilterd,
            ITradingSimulationConfig tradingSimulationConfig,
            IProgressReportable progress = null)
        {

            Balance = tradingSimulationConfig.StartingCash;
            OpenedPositions.Clear();
            TransactionsLedger.Clear();
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
                    var volume = (Balance / tradingSimulationConfig.TopN) / (stockQuoteForToday.Open);
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

            return result;
        }

        public virtual SimulationResult Simulate(ITradingSimulationConfig tradingSimulationConfig,
            IProgressReportable progress = null)
        {

            var allQuotesPrefilterd = _stockQuoteRepository
                .GetAll(x => !_projectSettings.BlackListPattern.IsMatch(x.Ticker)
                             && x.DateParsed.InOpenRange(tradingSimulationConfig.FromDate.AddDays(-30), tradingSimulationConfig.ToDate))
                .ToList();

            return Simulate(allQuotesPrefilterd, tradingSimulationConfig, progress);
        }

        public virtual List<StockQuote> GetSignals(ITradingSimulationConfig tradingSimulationConfig, DateTime date)
        {
            var allQuotesFromLastSession = _stockQuoteRepository.GetAllQuotesFromPreviousSession(date);
            return GetTopN(tradingSimulationConfig, allQuotesFromLastSession, date);
        }

        protected abstract List<StockQuote> GetTopN(ITradingSimulationConfig tradingSimulationConfig, List<StockQuote> allQuotesPrefilterd, DateTime date);

        public OrderStatusType PlaceBuyOrder(StockQuote quote, double price, double volume)
        {
            if (Balance * 1.001 <= price * volume)
            {
                _logger.LogWarning($"Buy order for {quote.Ticker} price = {price} vol = {volume} denied due to insufficient funds ({Balance}).");
                return OrderStatusType.DeniedInsufficientFunds;
            }
            if (!price.InOpenRange(quote.Low, quote.High))
            {
                _logger.LogWarning($"Buy order for {price * volume} denied due to price being out of range of today's {quote.Ticker} prices ({quote.Low} - {quote.High}.");
                return OrderStatusType.DeniedOutOfRange;
            }

            var isPositionOpened = OpenedPositions.ContainsKey(quote.Ticker);
            var newPosition = new OpenedPosition { Price = price, Volume = volume };
            if (isPositionOpened)
            {
                OpenedPositions[quote.Ticker] += newPosition;
            }
            else
            {
                OpenedPositions.Add(quote.Ticker, newPosition);
            }

            TransactionsLedger.Add(new StockTransaction
            {
                Ticker = quote.Ticker,
                Date = quote.DateParsed,
                Price = price,
                Volume = volume,
                TransactionType = StockTransactionType.Buy
            });

            UpdateBalance(price, volume, StockTransactionType.Buy);

            return OrderStatusType.Accepted;
        }

        private void UpdateBalance(double price, double volume, StockTransactionType transaction)
        {
            switch (transaction)
            {
                case StockTransactionType.Sell:
                    Balance += price * volume;
                    break;
                case StockTransactionType.Buy:
                    Balance -= price * volume;
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(transaction), transaction, null);
            }
        }

        public OrderStatusType ClosePosition(StockQuote quote, double price)
        {
            return PlaceSellOrder(quote, price, double.MaxValue);
        }

        public OrderStatusType PlaceSellOrder(StockQuote quote, double price, double volume)
        {
            if (!OpenedPositions.TryGetValue(quote.Ticker, out var openedPosition))
            {
                _logger.LogWarning($"Sell order of {volume} stocks denied due to lack of open position on {quote.Ticker} .");
                return OrderStatusType.DeniedNoOpenPosition;
            }

            if (!price.InOpenRange(quote.Low, quote.High))
            {
                _logger.LogWarning($"Sell order for of {volume} stocks denied due to price being out of range of today's {quote.Ticker} prices ({quote.Low} - {quote.High}).");
                return OrderStatusType.DeniedOutOfRange;
            }

            if (volume >= openedPosition.Volume)
            {
                volume = openedPosition.Volume;
                OpenedPositions.Remove(quote.Ticker);
            }
            else
            {
                openedPosition.Decrease(volume);
            }

            TransactionsLedger.Add(new StockTransaction
            {
                Ticker = quote.Ticker,
                Date = quote.DateParsed,
                Price = price,
                Volume = volume,
                TransactionType = StockTransactionType.Sell
            });

            UpdateBalance(price, volume, StockTransactionType.Sell);

            return OrderStatusType.Accepted;
        }
    }
}
