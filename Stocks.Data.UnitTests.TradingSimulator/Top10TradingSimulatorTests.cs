using Extensions.Standard;
using LoggerLite;
using Moq;
using MoreLinq;
using Stocks.Data.Common.Models;
using Stocks.Data.Ef;
using Stocks.Data.Model;
using Stocks.Data.TradingSimulator;
using Stocks.Data.TradingSimulator.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using Stocks.Data.Ef.Repositories;
using Xunit;

namespace Stocks.Data.UnitTests.TradingSimulator
{
    public class Top10TradingSimulatorTests
    {
        private readonly Mock<ILogger> _loggerMock = new Mock<ILogger>();
        private readonly Mock<IStockQuoteRepository> _stockQuoteRepositoryMock = new Mock<IStockQuoteRepository>();
        private readonly Mock<IProjectSettings> _projectSettingsMock = new Mock<IProjectSettings>();

        private TradingSimulationConfig TradingConfig => new TradingSimulationConfig
        {
            TopN = 10
        };

        private Top10TradingSimulator Tested => new Top10TradingSimulator(_loggerMock.Object, _stockQuoteRepositoryMock.Object, _projectSettingsMock.Object);

        [Theory]
        [ClassData(typeof(CompaniesMock))]
        public void GetSignalsGetsValidResultsCorrectlyOrdered(List<Company> testCompanies)
        {
            // Arrange
            var tested = Tested;
            var flattenedQuotes = testCompanies.SelectMany(x => x.Quotes).OrderBy(x => x.DateParsed).ToList();

            var tradingDate = new DateTime(2021, 02, 15);
            var last3dates = flattenedQuotes.Where(x => x.DateParsed < tradingDate)
                .Select(x => x.DateParsed)
                .Distinct()
                .OrderByDescending(x => x.Date)
                .Take(3)
                .ToList();
            _stockQuoteRepositoryMock.Setup(x => x.GetAllQuotesFromPreviousNDays(tradingDate, 3)).Returns(
                flattenedQuotes
                    .Where(x => x.DateParsed.InOpenRange(last3dates.Min(), last3dates.Max()))
                    .ToList());
            _stockQuoteRepositoryMock.Setup(x =>
                    x.GetAllQuotesFromPreviousSession(It.Is<DateTime>(z => z.Equals(tradingDate))))
                .Returns((DateTime current) => flattenedQuotes
                    .Where(f => f.DateParsed == new DateTime(2021, 02, 12))
                    .ToList());

            // Act
            var result = tested.GetSignals(TradingConfig, tradingDate);

            // Assert
            Assert.Equal(10, result.Count);
            for (var i = 0; i < result.Count; i++)
            {
                var quoteFromMinusOneDay = result[i];
                var allQuotesWithHigherAveragePriceChange = flattenedQuotes
                    .Where(x => x.DateParsed.Equals(quoteFromMinusOneDay.DateParsed)
                                && x.Ticker != quoteFromMinusOneDay.Ticker
                                && x.AveragePriceChange >= quoteFromMinusOneDay.AveragePriceChange)
                    .OrderByDescending(x => x.AveragePriceChange)
                    .ToList();
                Assert.True(i <= allQuotesWithHigherAveragePriceChange.Count);
                Assert.True(quoteFromMinusOneDay.DateParsed < tradingDate);
                Assert.True(quoteFromMinusOneDay.PreviousStockQuote.DateParsed < quoteFromMinusOneDay.DateParsed);
                Assert.True(quoteFromMinusOneDay.IsValid());
                var quoteFromMinusTwoDays = quoteFromMinusOneDay.PreviousStockQuote;
                Assert.True(quoteFromMinusTwoDays.IsValid());
                Assert.True(quoteFromMinusTwoDays.DateParsed < quoteFromMinusOneDay.DateParsed);
                Assert.Equal(quoteFromMinusOneDay.Ticker, quoteFromMinusTwoDays.Ticker);
                var expectedAvgPriceChange =
                    (quoteFromMinusOneDay.AveragePrice - quoteFromMinusTwoDays.AveragePrice) /
                    quoteFromMinusTwoDays.AveragePrice;
                Assert.Equal(expectedAvgPriceChange, quoteFromMinusOneDay.AveragePriceChange);
            }
        }

        [Theory]
        [ClassData(typeof(CompaniesMock))]
        public void SimulateTradingReturnsExpectedSimulationResult(List<Company> testCompanies)
        {
            // Arrange

            var tested = Tested;
            var flattenedQuotes = testCompanies.SelectMany(x => x.Quotes).OrderBy(x => x.DateParsed).ToList();
            var simulationConfig = new TradingSimulationConfig
            {
                FromDate = new DateTime(2020, 01, 01),
                ToDate = new DateTime(2021, 01, 01),
                StartingCash = 1000,
                TopN = 10
            };

            var allQuotesPrefiltered = flattenedQuotes.Where(z =>
                !new Regex(@".*\d{3,}|WIG.*|RC.*|INTL.*|INTS.*|WIG.*|.*PP\d.*|.*BAHOLDING.*|CFI.*").IsMatch(z.Ticker)
                && z.DateParsed.InOpenRange(simulationConfig.FromDate.AddDays(-30),
                    simulationConfig.ToDate))
                .ToList();
            // Act

            var result = tested.Simulate(allQuotesPrefiltered, simulationConfig);

            // Assert
            Assert.Equal(102.65, result.FinalBalance, 2);
            Assert.Equal(-89.73, result.ReturnOnInvestment, 2);
            Assert.Equal(0.33, result.ROC.Accuracy, 2);
            Assert.Equal(2394, result.ROC.All);
            Assert.True(result.TransactionsLedger.All(x => x.Date.InOpenRange(simulationConfig.FromDate, simulationConfig.ToDate)));
            Assert.Equal(result.ROC.All * 2, result.TransactionsLedger.Count);
            Assert.Equal(result.ROC.All, result.TransactionsLedger.Count(x => x.TransactionType == StockTransactionType.Sell));
            Assert.Equal(result.ROC.All, result.TransactionsLedger.Count(x => x.TransactionType == StockTransactionType.Buy));
        }
    }
}
