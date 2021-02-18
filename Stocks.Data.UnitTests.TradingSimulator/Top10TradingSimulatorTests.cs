using Extensions.Standard;
using LoggerLite;
using Moq;
using MoreLinq;
using Stocks.Data.Common.Models;
using Stocks.Data.Ef;
using Stocks.Data.Model;
using Stocks.Data.TradingSimulator;
using Stocks.Data.TradingSimulator.Models;
using Stocks.Data.UnitTests.TradingSimulator.TestData;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using Xunit;

namespace Stocks.Data.UnitTests.TradingSimulator
{
    public class Top10TradingSimulatorTests
    {
        private readonly Mock<ILogger> _loggerMock = new Mock<ILogger>();
        private readonly Mock<IStockQuoteRepository> _stockQuoteRepositoryMock = new Mock<IStockQuoteRepository>();
        private readonly Mock<IProjectSettings> _projectSettingsMock = new Mock<IProjectSettings>();

        [Theory]
        [ClassData(typeof(CompaniesMock))]
        public void GetTopNGetsValidResultsCorrectlyOrdered(List<Company> testCompanies)
        {
            // Arrange

            var flattenedQuotes = testCompanies.SelectMany(x => x.Quotes).OrderBy(x => x.DateParsed).ToList();
            var startDate = new DateTime(2021, 1, 10);
            var endDate = new DateTime(2021, 2, 12);

            var tradingDates = flattenedQuotes
                .Select(x => x.DateParsed)
                .Distinct()
                .Where(x => x.InOpenRange(startDate, endDate))
                .OrderBy(x => x);

            var tested = new Top10TradingSimulator(_loggerMock.Object, _stockQuoteRepositoryMock.Object, _projectSettingsMock.Object);

            // Arrange && Act & Assert

            foreach (var tradingDate in tradingDates)
            {
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
                        x.GetAllQuotesFromPreviousSession(It.Is<DateTime>(z => z.InOpenRange(startDate, endDate))))
                    .Returns((DateTime current) => flattenedQuotes
                        .Where(f => f.DateParsed == GetPreviousSessionDate(current, flattenedQuotes))
                        .ToList());

                var result = tested.GetSignals(tradingDate);
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
        }

        [Theory]
        [ClassData(typeof(CompaniesMock))]
        public void SimulateTradingReturnsExpectedSimulationResult(List<Company> testCompanies)
        {
            // Arrange

            var flattenedQuotes = testCompanies.SelectMany(x => x.Quotes).OrderBy(x => x.DateParsed).ToList();
            var tested = new Top10TradingSimulator(_loggerMock.Object, _stockQuoteRepositoryMock.Object, _projectSettingsMock.Object);
            var simulationConfig = new TradingSimulationConfig
            {
                FromDate = new DateTime(2020, 01, 01),
                ToDate = new DateTime(2021, 01, 01),
                StartingCash = 1000
            };
            _stockQuoteRepositoryMock.Setup(x => x.GetAll(It.IsAny<Expression<Func<StockQuote, bool>>>()))
                .Returns(flattenedQuotes.Where(z => !simulationConfig.BlackListPattern.IsMatch(z.Ticker)
                                                    && z.DateParsed.InOpenRange(simulationConfig.FromDate.AddDays(-30),
                                                        simulationConfig.ToDate)));
            //var currentDate = default(DateTime);
            _stockQuoteRepositoryMock.Setup(x =>
                    x.GetAllQuotesFromPreviousSession(It.Is<DateTime>(z => z.InOpenRange(simulationConfig.FromDate, simulationConfig.ToDate))))
                .Returns((DateTime current) => flattenedQuotes
                    .Where(f => f.DateParsed == GetPreviousSessionDate(current, flattenedQuotes))
                    .ToList());

            _stockQuoteRepositoryMock.Setup(x => x.GetTradingDates(simulationConfig.FromDate, simulationConfig.ToDate))
                .Returns(GetTradingDatesForConfig(simulationConfig, flattenedQuotes));
            // Act

            var result = tested.Simulate(simulationConfig);

            // Assert
            Assert.Equal(818.4145707444269, result.FinalBalance);
            Assert.Equal(-18.15854292555731, result.ReturnOnInvestment);
            Assert.Equal(0.36385639370713996, result.ROC.Accuracy);
            Assert.Equal(2479, result.ROC.All);
            Assert.True(result.TransactionsLedger.All(x => x.Date.InOpenRange(simulationConfig.FromDate, simulationConfig.ToDate)));
            Assert.Equal(result.ROC.All * 2, result.TransactionsLedger.Count);
            Assert.Equal(result.ROC.All, result.TransactionsLedger.Count(x => x.TransactionType == StockTransactionType.Sell));
            Assert.Equal(result.ROC.All, result.TransactionsLedger.Count(x => x.TransactionType == StockTransactionType.Buy));
        }

        private List<DateTime> GetTradingDatesForConfig(ITradingSimulationConfig config, List<StockQuote> quotes)
        {
            var dates = quotes.Where(x =>
                    x.DateParsed.InOpenRange(config.FromDate, config.ToDate))
                .Select(x => x.DateParsed)
                .Distinct()
                .OrderBy(x => x)
                .ToList();

            return dates;
        }

        private DateTime GetPreviousSessionDate(DateTime currentDate, List<StockQuote> flattenedQuotes)
        {
            var date = flattenedQuotes
                .Where(z => z.DateParsed < currentDate)
                .Select(y => y.DateParsed)
                .Distinct()
                .OrderByDescending(y => y)
                .Take(1)
                .Single();

            return date;
        }
    }
}
