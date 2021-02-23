using System;
using System.Linq;
using Microsoft.VisualStudio.TestPlatform.ObjectModel.DataCollector.InProcDataCollector;
using Stocks.Data.UnitTests.Ef.Test.TestData;
using Stocks.Data.Model;
using Xunit;
using Stocks.Data.Ef;
using Stocks.Data.Ef.Repositories;
using Stocks.Data.UnitTests.Ef.Test.Config;

namespace Stocks.Data.UnitTests.Ef.Test
{
    public class StockContextTest
    {
        [Theory]
        [ClassData(typeof(_11BitMock))]
        public void AddStock(Company company)
        {
            // Arrange
            var testSettings = new TestProjectSettings();
            StockContext testContext = null;
            Repository<Company> tested = null;
            try
            {
                testContext = new StockContext(testSettings);
                testContext.Database.EnsureCreated();
                tested = new Repository<Company>(testContext);
                testContext.Add(company);
                var changesCount = testContext.SaveChanges();

                // Act

                var actual = testContext.Set<Company>().Count();
                var expected = 1;

                // Assert

                Assert.Equal(expected, actual);
            }
            finally
            {
                testContext?.Database.EnsureDeleted();
                tested?.Dispose();
                testContext?.Dispose();
            }
        }

        [Theory]
        [ClassData(typeof(_11BitMock))]
        public void RemoveSpecificStock(Company company)
        {
            // Arrange

            var testSettings = new TestProjectSettings();

            StockContext testContext = null;
            Repository<Company> tested = null;
            try
            {
                testContext = new StockContext(testSettings);
                testContext.Database.EnsureCreated();
                tested = new Repository<Company>(testContext);
                testContext.Add(company);
                var changesCount = testContext.SaveChanges();

                // Act

                tested.Remove(company);
                var changesCount2 = testContext.SaveChanges();


                var actual = testContext.Set<Company>().Count();
                var expected = 0;

                // Assert

                Assert.Equal(expected, actual);
            }
            finally
            {
                testContext?.Database.EnsureDeleted();
                tested?.Dispose();
                testContext?.Dispose();
            }
        }

        [Fact]
        public void RemoveSpecificStockThrowsWhenNull()
        {
            // Arrange
            
            var testSettings = new TestProjectSettings();

            StockContext testContext = null;
            Repository<Company> tested = null;
            try
            {
                testContext = new StockContext(testSettings);
                testContext.Database.EnsureCreated();
                tested = new Repository<Company>(testContext);

                // Act & Assert

                Assert.Throws<ArgumentNullException>(() => tested.Remove(null));
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
