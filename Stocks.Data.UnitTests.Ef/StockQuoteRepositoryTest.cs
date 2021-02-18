using Microsoft.EntityFrameworkCore;
using Stocks.Data.Ef;
using Stocks.Data.Model;
using Stocks.Data.UnitTests.Ef.Test.Config;
using Stocks.Data.UnitTests.Ef.Test.TestData;
using System;
using System.Collections.Generic;
using System.Linq;
using Xunit;

namespace Stocks.Data.UnitTests.Ef.Test
{
    public class StockQuoteRepositoryTest
    {
        [Theory]
        [ClassData(typeof(_11BitMock))]
        public void AddAdds(Company input)
        {
            // Arrange
            var testSettings = new TestProjectSettings();

            DbContext testContext = null;
            StockQuoteRepository tested = null;
            try
            {
                testContext = new StockTestContext(testSettings);
                tested = new StockQuoteRepository(testContext);
                testContext.Database.EnsureCreated();

                // Act

                testContext.Add(input.FirstQuote);
                testContext.SaveChanges();

                // Assert

                Assert.Equal(input.FirstQuote, testContext.Set<StockQuote>().First());
            }
            finally
            {
                testContext?.Database.EnsureDeleted();
                tested?.Dispose();
                testContext?.Dispose();
            }
        }

        [Theory]
        [ClassData(typeof(CompaniesMock))]
        public void GetTradingDatesGetsOnlyTradingDatesInOpenRangeOrderedAscending(List<Company> input)
        {
            // Arrange
            var testSettings = new TestProjectSettings();

            DbContext testContext = null;
            StockQuoteRepository tested = null;
            try
            {
                testContext = new StockTestContext(testSettings);
                tested = new StockQuoteRepository(testContext);
                testContext.Database.EnsureCreated();
                testContext.AddRange(input.SelectMany(x => x.Quotes));
                testContext.SaveChanges();

                // Act
                var result = tested.GetTradingDates(new DateTime(2017, 01, 01), new DateTime(2018, 01, 01));

                // Assert

                Assert.Equal(250, result.Count);
                for (var i = 1; i < result.Count; i++)
                {
                    Assert.True(result[i] > result[i - 1]);
                }
            }
            finally
            {
                testContext?.Database.EnsureDeleted();
                tested?.Dispose();
                testContext?.Dispose();
            }
        }
        
        [Theory]
        [ClassData(typeof(CompaniesMock))]
        public void GetNTradingDatesBeforeGetsOnlyTradingDatesInOpenRangeOrderedDescending(List<Company> input)
        {
            // Arrange
            var testSettings = new TestProjectSettings();

            DbContext testContext = null;
            StockQuoteRepository tested = null;
            try
            {
                testContext = new StockTestContext(testSettings);
                tested = new StockQuoteRepository(testContext);
                testContext.Database.EnsureCreated();
                testContext.AddRange(input.SelectMany(x => x.Quotes));
                testContext.SaveChanges();
                var date = new DateTime(2017, 01, 01);

                // Act
                var result = tested.GetNTradingDatesBefore(date, 10);

                // Assert

                Assert.Equal(10, result.Count);
                Assert.DoesNotContain(result, x => x.Equals(date));
                for (var i = 1; i < result.Count; i++)
                {
                    Assert.True(result[i] < result[i - 1]);
                }
            }
            finally
            {
                testContext?.Database.EnsureDeleted();
                tested?.Dispose();
                testContext?.Dispose();
            }
        }

        [Theory]
        [ClassData(typeof(CompaniesMock))]
        public void GetNTradingDatesGetsOnlyTradingDatesFromLastNSessionsOrderedDescending(List<Company> input)
        {
            // Arrange
            var testSettings = new TestProjectSettings();

            DbContext testContext = null;
            StockQuoteRepository tested = null;
            try
            {
                testContext = new StockTestContext(testSettings);
                tested = new StockQuoteRepository(testContext);
                testContext.Database.EnsureCreated();
                testContext.AddRange(input.Where(x => x.Ticker == "MBANK").SelectMany(x => x.Quotes));
                testContext.SaveChanges();

                // Act
                var result = tested.GetAllQuotesFromPreviousNDays(new DateTime(2017, 01, 01), 10);

                // Assert

                Assert.Equal(10, result.Count);
                for (var i = 1; i < result.Count; i++)
                {
                    Assert.True(result[i].DateParsed > result[i - 1].DateParsed);
                }
            }
            finally
            {
                testContext?.Database.EnsureDeleted();
                tested?.Dispose();
                testContext?.Dispose();
            }
        }

        [Theory]
        [ClassData(typeof(CompaniesMock))]
        public void GetAllQuotesFromPreviousSessionGetsOnlyQuotesFromLastSession(List<Company> input)
        {
            // Arrange
            var testSettings = new TestProjectSettings();

            DbContext testContext = null;
            StockQuoteRepository tested = null;
            try
            {
                testContext = new StockTestContext(testSettings);
                tested = new StockQuoteRepository(testContext);
                testContext.Database.EnsureCreated();
                testContext.AddRange(input.SelectMany(x => x.Quotes));
                testContext.SaveChanges();
                var date = new DateTime(2017, 01, 01);
                // Act
                var result = tested.GetAllQuotesFromPreviousSession(date);

                // Assert

                Assert.Equal(3, result.Count);
                foreach (var quote in result)
                {
                    Assert.Equal(new DateTime(2016, 12, 30), quote.DateParsed);
                }
            }
            finally
            {
                testContext?.Database.EnsureDeleted();
                tested?.Dispose();
                testContext?.Dispose();
            }
        }
    }
}
