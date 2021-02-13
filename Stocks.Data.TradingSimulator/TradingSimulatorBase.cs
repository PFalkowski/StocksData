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
        public readonly Dictionary<string, (double price, double volume)> OpenedPositions = new Dictionary<string, (double price, double volume)>();
        public readonly List<StockTransaction> TransactionsLedger = new List<StockTransaction>();

        public bool PlaceBuyOrder(StockQuote quote, double price, double volume)
        {
            if (Balance < price * volume)
            {
                return false;
            }
            if (!price.InClosedRange(quote.Low, quote.High))
            {
                return false;
            }

            var isPositionOpened = OpenedPositions.TryGetValue(quote.Ticker, out var openedPosition);

            if (isPositionOpened)
            {
                openedPosition.price = (openedPosition.price * openedPosition.volume + price * volume)
                    / (openedPosition.volume + volume);
                openedPosition.volume += volume;
            }
            else
            {
                OpenedPositions.Add(quote.Ticker, (price: price, volume: volume));
            }

            TransactionsLedger.Add(new StockTransaction
            {
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

            if (!price.InClosedRange(quote.Low, quote.High))
            {
                return false;
            }
            
            if (volume >= openedPosition.volume)
            {
                volume = openedPosition.volume;
                OpenedPositions.Remove(quote.Ticker);
            }
            else
            {
                openedPosition.volume -= volume;
            }

            TransactionsLedger.Add(new StockTransaction
            {
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
