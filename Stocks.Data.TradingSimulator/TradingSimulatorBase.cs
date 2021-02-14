using System;
using Extensions.Standard;
using Stocks.Data.Model;
using Stocks.Data.TradingSimulator.Models;
using System.Collections.Generic;

namespace Stocks.Data.TradingSimulator
{
    public class TradingSimulatorBase
    {
        public double Balance { get; protected set; }
        public readonly Dictionary<string, OpenedPosition> OpenedPositions = new Dictionary<string, OpenedPosition>();
        public readonly List<StockTransaction> TransactionsLedger = new List<StockTransaction>();

        public bool PlaceBuyOrder(StockQuote quote, double price, double volume)
        {
            if (Balance < price * volume)
            {
                return false;
            }
            if (!price.InOpenRange(quote.Low, quote.High))
            {
                return false;
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

            return true;
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

        public bool ClosePosition(StockQuote quote, double price)
        {
            return PlaceSellOrder(quote, price, double.MaxValue);
        }

        public bool PlaceSellOrder(StockQuote quote, double price, double volume)
        {
            if (!OpenedPositions.TryGetValue(quote.Ticker, out var openedPosition))
            {
                return false;
            }

            if (!price.InOpenRange(quote.Low, quote.High))
            {
                return false;
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

            return true;
        }
    }
}
