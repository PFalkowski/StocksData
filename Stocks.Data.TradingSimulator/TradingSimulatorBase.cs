using System;
using Extensions.Standard;
using Stocks.Data.Model;
using Stocks.Data.TradingSimulator.Models;
using System.Collections.Generic;
using LoggerLite;
using ProgressReporting;

namespace Stocks.Data.TradingSimulator
{
    public class TradingSimulatorBase
    {
        public double Balance { get; protected set; }
        public readonly Dictionary<string, OpenedPosition> OpenedPositions = new Dictionary<string, OpenedPosition>();
        public readonly List<StockTransaction> TransactionsLedger = new List<StockTransaction>();
        protected readonly ILogger _logger;

        public TradingSimulatorBase(ILogger logger)
        {
            _logger = logger;
        }

        public virtual SimulationResult Simulate(ITradingSimulationConfig tradingSimulationConfig,
            IProgressReportable progress = null)
        {
            Balance = tradingSimulationConfig.StartingCash;
            OpenedPositions.Clear();
            TransactionsLedger.Clear();

            return new SimulationResult();
        }

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
            var newPosition = new OpenedPosition {Price = price, Volume = volume};
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
