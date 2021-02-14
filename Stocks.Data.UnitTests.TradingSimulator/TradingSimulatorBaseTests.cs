using System.Linq;
using NUnit.Framework;
using Stocks.Data.Model;
using Stocks.Data.TradingSimulator;
using Stocks.Data.TradingSimulator.Models;

namespace Stocks.Data.UnitTests.TradingSimulator
{
    public class TradingSimulatorBaseTests
    {
        private const string Ticker = nameof(Ticker);
        private StockQuote quote1 = new StockQuote
        {
            Close = 11,
            Date = 20001010,
            Ticker = Ticker,
            High = 14,
            Low = 6,
            Volume = 1000,
            Open = 10,
            TotalSharesEmitted = 100000,
            MarketCap = 200000,
            BookValue = 1000,
            DividendYield = 1.0,
            PriceToEarningsRatio = 2
        };
        private StockQuote quote2 = new StockQuote
        {
            Close = 9,
            Date = 20001011,
            Ticker = Ticker,
            High = 11,
            Low = 6,
            Volume = 700,
            Open = 11,
            TotalSharesEmitted = 100000,
            MarketCap = 200000,
            BookValue = 1000,
            DividendYield = 1.0,
            PriceToEarningsRatio = 2
        };

        [SetUp]
        public void Setup()
        {
        }

        [Test]
        public void PlaceBuyOrderDoesNotPlaceOrderWhenBalanceIsTooLow()
        {
            // Arrange

            const int volume = 10;
            const int buyPrice = 11;
            var tested = new TradingSimulatorBase();

            // Act

            var result = tested.PlaceBuyOrder(quote1, buyPrice, volume);

            // Assert
            Assert.Multiple(() =>
            {
                Assert.False(result);
                Assert.False(tested.TransactionsLedger.Any());
                Assert.False(tested.OpenedPositions.Any());
            });
        }

        class SetBalanceOnTestedObject : TradingSimulatorBase
        {
            public SetBalanceOnTestedObject(double balance)
            {
                Balance = balance;
            }
        }

        [Test]
        public void PlaceBuyOrderDoesNotPlaceOrderWhenPriceIsOutOfRange()
        {
            // Arrange

            const int volume = 10;
            const double buyPrice = 14.01;
            const double balance = 1000;
            var tested = new SetBalanceOnTestedObject(balance);
            // Act

            var result = tested.PlaceBuyOrder(quote1, buyPrice, volume);

            // Assert
            Assert.Multiple(() =>
            {
                Assert.False(result);
                Assert.False(tested.TransactionsLedger.Any());
                Assert.False(tested.OpenedPositions.Any());
            });
        }

        [Test]
        public void PlaceBuyOrderPlacesCorrectOrder()
        {
            // Arrange

            const int volume = 10;
            const int buyPrice = 11;
            const double balance = 1000;
            var tested = new SetBalanceOnTestedObject(balance);
            // Act

            var result = tested.PlaceBuyOrder(quote1, buyPrice, volume);

            // Assert
            Assert.Multiple(() =>
            {
                Assert.True(result);
                Assert.AreEqual(balance - buyPrice * volume, tested.Balance);

                Assert.AreEqual(Ticker, tested.TransactionsLedger.Single().Ticker);
                Assert.AreEqual(quote1.DateParsed, tested.TransactionsLedger.Single().Date);
                Assert.AreEqual(buyPrice, tested.TransactionsLedger.Single().Price);
                Assert.AreEqual(volume, tested.TransactionsLedger.Single().Volume);
                Assert.AreEqual(StockTransactionType.Buy, tested.TransactionsLedger.Single().TransactionType);
                Assert.AreEqual(Ticker, tested.OpenedPositions.Single().Key);
                Assert.AreEqual(buyPrice, tested.OpenedPositions.Single().Value.Price);
                Assert.AreEqual(volume, tested.OpenedPositions.Single().Value.Volume);
            });
        }
        
        [Test]
        public void PlaceBuyOrderPlacesCorrectOrderWhenPositionIsOpenedAlready()
        {
            // Arrange

            const int volume = 10;
            const int volume2 = 10;
            const int buyPrice = 11;
            const int buyPrice2 = 11;
            const double balance = 1000;
            var tested = new SetBalanceOnTestedObject(balance);
            // Act

            var result = tested.PlaceBuyOrder(quote1, buyPrice, volume);
            var result2 = tested.PlaceBuyOrder(quote2, buyPrice2, volume2);

            // Assert
            Assert.Multiple(() =>
            {
                Assert.True(result);
                Assert.True(result2);
                Assert.AreEqual(balance - buyPrice * volume * 2, tested.Balance);

                Assert.AreEqual(Ticker, tested.TransactionsLedger.First().Ticker);
                Assert.AreEqual(Ticker, tested.TransactionsLedger.Last().Ticker);

                Assert.AreEqual(quote1.DateParsed, tested.TransactionsLedger.First().Date);
                Assert.AreEqual(quote2.DateParsed, tested.TransactionsLedger.Last().Date);

                Assert.AreEqual(buyPrice, tested.TransactionsLedger.First().Price);
                Assert.AreEqual(buyPrice2, tested.TransactionsLedger.Last().Price);

                Assert.AreEqual(volume, tested.TransactionsLedger.First().Volume);
                Assert.AreEqual(volume2, tested.TransactionsLedger.Last().Volume);

                Assert.AreEqual(StockTransactionType.Buy, tested.TransactionsLedger.First().TransactionType);
                Assert.AreEqual(StockTransactionType.Buy, tested.TransactionsLedger.Last().TransactionType);

                Assert.AreEqual(Ticker, tested.OpenedPositions.First().Key);
                Assert.AreEqual(Ticker, tested.OpenedPositions.Last().Key);

                Assert.AreEqual(11, tested.OpenedPositions.Single().Value.Price);

                Assert.AreEqual(volume + volume2, tested.OpenedPositions.Single().Value.Volume);
            });
        }

        [Test]
        public void PlaceBuyOrderAveragesByWeightBalance()
        {
            // Arrange

            const int volume = 5;
            const int volume2 = 10;
            const int buyPrice = 11;
            const int buyPrice2 = 10;
            const double balance = 1000;
            var tested = new SetBalanceOnTestedObject(balance);
            // Act

            var result = tested.PlaceBuyOrder(quote1, buyPrice, volume);
            var result2 = tested.PlaceBuyOrder(quote2, buyPrice2, volume2);

            // Assert
            Assert.Multiple(() =>
            {
                Assert.True(result);
                Assert.True(result2);
                Assert.AreEqual(10.333333333333334d, tested.OpenedPositions.Single().Value.Price);
                Assert.AreEqual(15, tested.OpenedPositions.Single().Value.Volume);
                Assert.AreEqual(845d, tested.Balance);
            });
        }

        [Test]
        public void PlaceSellOrderDoesNotPlaceOrderWhenNoOpenedPosition()
        {
            // Arrange

            const int volume = 10;
            const double sellPrice = 11;
            var tested = new TradingSimulatorBase();

            // Act
            
            var result = tested.PlaceSellOrder(quote1, sellPrice, volume);

            // Assert
            Assert.Multiple(() =>
            {
                Assert.False(result);
                Assert.False(tested.TransactionsLedger.Any());
                Assert.False(tested.OpenedPositions.Any());
            });
        }
        
        [Test]
        public void PlaceSellOrderDoesNotPlaceOrderWhenPriceIsOutOfRange()
        {
            // Arrange

            const int volume = 10;
            const double sellPrice = 14.01;
            const double balance = 1000;
            var tested = new SetBalanceOnTestedObject(balance);
            // Act

            var result = tested.PlaceSellOrder(quote1, sellPrice, volume);

            // Assert
            Assert.Multiple(() =>
            {
                Assert.False(result);
                Assert.False(tested.TransactionsLedger.Any());
                Assert.False(tested.OpenedPositions.Any());
            });
        }
        
        [Test]
        public void PlaceSellClosesPositionWhenVolumeEqualToOpenedPos()
        {
            // Arrange

            const int buyVolume = 10;
            const int sellVolume = 10;
            const int buyPrice = 11;
            const int sellPrice = 11;
            const double balance = 1000;
            var tested = new SetBalanceOnTestedObject(balance);
            // Act
            
            var result = tested.PlaceBuyOrder(quote1, buyPrice, buyVolume);
            var result2 = tested.PlaceSellOrder(quote2, sellPrice, sellVolume);

            // Assert
            Assert.Multiple(() =>
            {
                Assert.True(result);
                Assert.True(result2);
                Assert.AreEqual(balance, tested.Balance);

                Assert.AreEqual(Ticker, tested.TransactionsLedger.First().Ticker);
                Assert.AreEqual(Ticker, tested.TransactionsLedger.Last().Ticker);

                Assert.AreEqual(quote1.DateParsed, tested.TransactionsLedger.First().Date);
                Assert.AreEqual(quote2.DateParsed, tested.TransactionsLedger.Last().Date);

                Assert.AreEqual(buyPrice, tested.TransactionsLedger.First().Price);
                Assert.AreEqual(sellPrice, tested.TransactionsLedger.Last().Price);

                Assert.AreEqual(buyVolume, tested.TransactionsLedger.First().Volume);
                Assert.AreEqual(sellVolume, tested.TransactionsLedger.Last().Volume);

                Assert.AreEqual(StockTransactionType.Buy, tested.TransactionsLedger.First().TransactionType);
                Assert.AreEqual(StockTransactionType.Sell, tested.TransactionsLedger.Last().TransactionType);
                
                Assert.False(tested.OpenedPositions.Any());
            });
        }

        [Test]
        public void ClosePositionClosesPosition()
        {
            // Arrange

            const int buyVolume = 10;
            const int sellVolume = 10;
            const int buyPrice = 11;
            const int sellPrice = 11;
            const double balance = 1000;
            var tested = new SetBalanceOnTestedObject(balance);
            // Act
            
            var result = tested.PlaceBuyOrder(quote1, buyPrice, buyVolume);
            var result2 = tested.ClosePosition(quote2, sellPrice);

            // Assert
            Assert.Multiple(() =>
            {
                Assert.True(result);
                Assert.True(result2);
                Assert.AreEqual(balance, tested.Balance);

                Assert.AreEqual(Ticker, tested.TransactionsLedger.First().Ticker);
                Assert.AreEqual(Ticker, tested.TransactionsLedger.Last().Ticker);

                Assert.AreEqual(quote1.DateParsed, tested.TransactionsLedger.First().Date);
                Assert.AreEqual(quote2.DateParsed, tested.TransactionsLedger.Last().Date);

                Assert.AreEqual(buyPrice, tested.TransactionsLedger.First().Price);
                Assert.AreEqual(sellPrice, tested.TransactionsLedger.Last().Price);

                Assert.AreEqual(buyVolume, tested.TransactionsLedger.First().Volume);
                Assert.AreEqual(sellVolume, tested.TransactionsLedger.Last().Volume);

                Assert.AreEqual(StockTransactionType.Buy, tested.TransactionsLedger.First().TransactionType);
                Assert.AreEqual(StockTransactionType.Sell, tested.TransactionsLedger.Last().TransactionType);
                
                Assert.False(tested.OpenedPositions.Any());
            });
        }
        
        [Test]
        public void PlaceSellOrderPlacesCorrectOrder()
        {
            // Arrange

            const int buyVolume = 10;
            const int sellVolume = 7;
            const int buyPrice = 11;
            const int sellPrice = 10;
            const double balance = 1000;
            var tested = new SetBalanceOnTestedObject(balance);
            // Act
            
            var result = tested.PlaceBuyOrder(quote1, buyPrice, buyVolume);
            var result2 = tested.PlaceSellOrder(quote2, sellPrice, sellVolume);

            // Assert
            Assert.Multiple(() =>
            {
                Assert.True(result);
                Assert.True(result2);
                Assert.AreEqual(balance - (3 * 11 + 7), tested.Balance);

                Assert.AreEqual(Ticker, tested.TransactionsLedger.First().Ticker);
                Assert.AreEqual(Ticker, tested.TransactionsLedger.Last().Ticker);

                Assert.AreEqual(quote1.DateParsed, tested.TransactionsLedger.First().Date);
                Assert.AreEqual(quote2.DateParsed, tested.TransactionsLedger.Last().Date);

                Assert.AreEqual(buyPrice, tested.TransactionsLedger.First().Price);
                Assert.AreEqual(sellPrice, tested.TransactionsLedger.Last().Price);

                Assert.AreEqual(buyVolume, tested.TransactionsLedger.First().Volume);
                Assert.AreEqual(sellVolume, tested.TransactionsLedger.Last().Volume);

                Assert.AreEqual(StockTransactionType.Buy, tested.TransactionsLedger.First().TransactionType);
                Assert.AreEqual(StockTransactionType.Sell, tested.TransactionsLedger.Last().TransactionType);
                
                Assert.AreEqual(11, tested.OpenedPositions.Single().Value.Price);

                Assert.AreEqual(buyVolume - sellVolume, tested.OpenedPositions.Single().Value.Volume);
            });
        }
    }
}