using Extensions.Standard;
using Stocks.Data.Model;
using System;
using System.Collections.Generic;

namespace Stocks.Data.TradingSimulator.Models
{
    public class TransactionsLedger
    {
        public double Balance { get; protected set; }
        public Dictionary<string, OpenedPosition> OpenedPositions { get; protected set; } = new Dictionary<string, OpenedPosition>();
        public List<StockTransaction> TheLedger { get; protected set; } = new List<StockTransaction>();

        public TransactionsLedger(double initialBalance)
        {
            Balance = initialBalance;
        }
        
        public OrderStatusType PlaceBuyOrder(StockQuote quote, double price, double volume)
        {
            if (Balance * 1.001 <= price * volume)
            {
                return OrderStatusType.DeniedInsufficientFunds;
            }
            if (!price.InOpenRange(quote.Low, quote.High))
            {
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

            TheLedger.Add(new StockTransaction
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

        public OrderStatusType ClosePosition(StockQuote quote, double price)
        {
            return PlaceSellOrder(quote, price, double.MaxValue);
        }

        public OrderStatusType PlaceSellOrder(StockQuote quote, double price, double volume)
        {
            if (!OpenedPositions.TryGetValue(quote.Ticker, out var openedPosition))
            {
                return OrderStatusType.DeniedNoOpenPosition;
            }

            if (!price.InOpenRange(quote.Low, quote.High))
            {
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

            TheLedger.Add(new StockTransaction
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
    }
}
