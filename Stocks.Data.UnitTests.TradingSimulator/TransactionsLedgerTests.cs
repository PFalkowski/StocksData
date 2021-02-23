using System;
using System.Collections.Generic;
using System.Linq;
using AutoMapper;
using LoggerLite;
using Moq;
using NUnit.Framework;
using Stocks.Data.Common.Models;
using Stocks.Data.Ef;
using Stocks.Data.Ef.Repositories;
using Stocks.Data.Model;
using Stocks.Data.TradingSimulator;
using Stocks.Data.TradingSimulator.Mapping;
using Stocks.Data.TradingSimulator.Models;

namespace Stocks.Data.UnitTests.TradingSimulator
{
    public class TransactionsLedgerTests
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

        private class SetBalanceOnTestedObject : TradingSimulatorBase
        {
            public SetBalanceOnTestedObject(ILogger logger,
                IStockQuoteRepository stockQuoteRepository,
                IProjectSettings projectSettings)
                : base(logger, stockQuoteRepository, projectSettings)
            {
            }

            protected override List<StockQuote> GetTopN(TradingSimulationConfig tradingSimulationConfig, List<StockQuote> allQuotesPrefilterd, DateTime date)
            {
                throw new NotImplementedException();
            }
        }

        private const double InitialBalance = 1000;

        private TransactionsLedger _tested => new TransactionsLedger(InitialBalance);

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
            var tested = new TransactionsLedger(0);

            // Act

            var result = tested.PlaceBuyOrder(quote1, buyPrice, volume);

            // Assert
            Assert.Multiple(() =>
            {
                Assert.AreEqual(OrderStatusType.DeniedInsufficientFunds, result);
                Assert.False(tested.TheLedger.Any());
                Assert.False(tested.OpenedPositions.Any());
            });
        }
        [Test]
        public void PlaceBuyOrderDoesNotPlaceOrderWhenPriceIsOutOfRange()
        {
            // Arrange

            const int volume = 10;
            const double buyPrice = 14.01;
            // Act
            var tested = _tested;
            var result = tested.PlaceBuyOrder(quote1, buyPrice, volume);

            // Assert
            Assert.Multiple(() =>
            {
                Assert.AreEqual(OrderStatusType.DeniedOutOfRange, result);
                Assert.False(tested.TheLedger.Any());
                Assert.False(tested.OpenedPositions.Any());
            });
        }

        [Test]
        public void PlaceBuyOrderPlacesCorrectOrder()
        {
            // Arrange

            const int volume = 10;
            const int buyPrice = 11;
            var tested = _tested;
            // Act

            var result = tested.PlaceBuyOrder(quote1, buyPrice, volume);

            // Assert
            Assert.Multiple(() =>
            {
                Assert.AreEqual(OrderStatusType.Accepted, result);
                Assert.AreEqual(_tested.Balance - buyPrice * volume, tested.Balance);

                Assert.AreEqual(Ticker, tested.TheLedger.Single().Ticker);
                Assert.AreEqual(quote1.DateParsed, tested.TheLedger.Single().Date);
                Assert.AreEqual(buyPrice, tested.TheLedger.Single().Price);
                Assert.AreEqual(volume, tested.TheLedger.Single().Volume);
                Assert.AreEqual(StockTransactionType.Buy, tested.TheLedger.Single().TransactionType);
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
            var tested = _tested;
            // Act

            var result = tested.PlaceBuyOrder(quote1, buyPrice, volume);
            var result2 = tested.PlaceBuyOrder(quote2, buyPrice2, volume2);

            // Assert
            Assert.Multiple(() =>
            {
                Assert.AreEqual(OrderStatusType.Accepted, result);
                Assert.AreEqual(OrderStatusType.Accepted, result2);
                Assert.AreEqual(_tested.Balance - buyPrice * volume * 2, tested.Balance);

                Assert.AreEqual(Ticker, tested.TheLedger.First().Ticker);
                Assert.AreEqual(Ticker, tested.TheLedger.Last().Ticker);

                Assert.AreEqual(quote1.DateParsed, tested.TheLedger.First().Date);
                Assert.AreEqual(quote2.DateParsed, tested.TheLedger.Last().Date);

                Assert.AreEqual(buyPrice, tested.TheLedger.First().Price);
                Assert.AreEqual(buyPrice2, tested.TheLedger.Last().Price);

                Assert.AreEqual(volume, tested.TheLedger.First().Volume);
                Assert.AreEqual(volume2, tested.TheLedger.Last().Volume);

                Assert.AreEqual(StockTransactionType.Buy, tested.TheLedger.First().TransactionType);
                Assert.AreEqual(StockTransactionType.Buy, tested.TheLedger.Last().TransactionType);

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
            var tested = _tested;
            // Act

            var result = tested.PlaceBuyOrder(quote1, buyPrice, volume);
            var result2 = tested.PlaceBuyOrder(quote2, buyPrice2, volume2);

            // Assert
            Assert.Multiple(() =>
            {
                Assert.AreEqual(OrderStatusType.Accepted, result);
                Assert.AreEqual(OrderStatusType.Accepted, result2);
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
            var tested = _tested;

            // Act

            var result = tested.PlaceSellOrder(quote1, sellPrice, volume);

            // Assert
            Assert.Multiple(() =>
            {
                Assert.AreEqual(OrderStatusType.DeniedNoOpenPosition, result);
                Assert.False(tested.TheLedger.Any());
                Assert.False(tested.OpenedPositions.Any());
            });
        }

        [Test]
        public void PlaceSellOrderDoesNotPlaceOrderWhenPriceIsOutOfRange()
        {
            // Arrange

            const int buyVolume = 10;
            const int buyPrice = 11;
            const int volume = 10;
            const double sellPrice = 14.01;
            var tested = _tested;
            // Act

            var result = tested.PlaceBuyOrder(quote1, buyPrice, buyVolume);
            var result2 = tested.PlaceSellOrder(quote1, sellPrice, volume);

            // Assert
            Assert.Multiple(() =>
            {
                Assert.AreEqual(OrderStatusType.DeniedOutOfRange, result2);
                Assert.AreEqual(1, tested.TheLedger.Count);
                Assert.AreEqual(1, tested.OpenedPositions.Count);
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
            var tested = _tested;
            // Act

            var result = tested.PlaceBuyOrder(quote1, buyPrice, buyVolume);
            var result2 = tested.PlaceSellOrder(quote2, sellPrice, sellVolume);

            // Assert
            Assert.Multiple(() =>
            {
                Assert.AreEqual(OrderStatusType.Accepted, result);
                Assert.AreEqual(OrderStatusType.Accepted, result2);
                Assert.AreEqual(_tested.Balance, tested.Balance);

                Assert.AreEqual(Ticker, tested.TheLedger.First().Ticker);
                Assert.AreEqual(Ticker, tested.TheLedger.Last().Ticker);

                Assert.AreEqual(quote1.DateParsed, tested.TheLedger.First().Date);
                Assert.AreEqual(quote2.DateParsed, tested.TheLedger.Last().Date);

                Assert.AreEqual(buyPrice, tested.TheLedger.First().Price);
                Assert.AreEqual(sellPrice, tested.TheLedger.Last().Price);

                Assert.AreEqual(buyVolume, tested.TheLedger.First().Volume);
                Assert.AreEqual(sellVolume, tested.TheLedger.Last().Volume);

                Assert.AreEqual(StockTransactionType.Buy, tested.TheLedger.First().TransactionType);
                Assert.AreEqual(StockTransactionType.Sell, tested.TheLedger.Last().TransactionType);

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
            var tested = _tested;
            // Act

            var result = tested.PlaceBuyOrder(quote1, buyPrice, buyVolume);
            var result2 = tested.ClosePosition(quote2, sellPrice);

            // Assert
            Assert.Multiple(() =>
            {
                Assert.AreEqual(OrderStatusType.Accepted, result);
                Assert.AreEqual(OrderStatusType.Accepted, result2);
                Assert.AreEqual(_tested.Balance, tested.Balance);

                Assert.AreEqual(Ticker, tested.TheLedger.First().Ticker);
                Assert.AreEqual(Ticker, tested.TheLedger.Last().Ticker);

                Assert.AreEqual(quote1.DateParsed, tested.TheLedger.First().Date);
                Assert.AreEqual(quote2.DateParsed, tested.TheLedger.Last().Date);

                Assert.AreEqual(buyPrice, tested.TheLedger.First().Price);
                Assert.AreEqual(sellPrice, tested.TheLedger.Last().Price);

                Assert.AreEqual(buyVolume, tested.TheLedger.First().Volume);
                Assert.AreEqual(sellVolume, tested.TheLedger.Last().Volume);

                Assert.AreEqual(StockTransactionType.Buy, tested.TheLedger.First().TransactionType);
                Assert.AreEqual(StockTransactionType.Sell, tested.TheLedger.Last().TransactionType);

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
            var tested = _tested;
            // Act

            var result = tested.PlaceBuyOrder(quote1, buyPrice, buyVolume);
            var result2 = tested.PlaceSellOrder(quote2, sellPrice, sellVolume);

            // Assert
            Assert.Multiple(() =>
            {
                Assert.AreEqual(OrderStatusType.Accepted, result);
                Assert.AreEqual(OrderStatusType.Accepted, result2);
                Assert.AreEqual(_tested.Balance - (3 * 11 + 7), tested.Balance);

                Assert.AreEqual(Ticker, tested.TheLedger.First().Ticker);
                Assert.AreEqual(Ticker, tested.TheLedger.Last().Ticker);

                Assert.AreEqual(quote1.DateParsed, tested.TheLedger.First().Date);
                Assert.AreEqual(quote2.DateParsed, tested.TheLedger.Last().Date);

                Assert.AreEqual(buyPrice, tested.TheLedger.First().Price);
                Assert.AreEqual(sellPrice, tested.TheLedger.Last().Price);

                Assert.AreEqual(buyVolume, tested.TheLedger.First().Volume);
                Assert.AreEqual(sellVolume, tested.TheLedger.Last().Volume);

                Assert.AreEqual(StockTransactionType.Buy, tested.TheLedger.First().TransactionType);
                Assert.AreEqual(StockTransactionType.Sell, tested.TheLedger.Last().TransactionType);

                Assert.AreEqual(11, tested.OpenedPositions.Single().Value.Price);

                Assert.AreEqual(buyVolume - sellVolume, tested.OpenedPositions.Single().Value.Volume);
            });
        }
    }
}