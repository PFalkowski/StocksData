using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Stocks.Data.Ef.Test.Mocks;
using Stocks.Data.Model;
using Xunit;

namespace Stocks.Data.Ef.Test
{
    public class StockContextTest
    {
        [Theory]
        [ClassData(typeof(_11BitMock))]
        public void AddStock(Company company)
        {
            var options = Config.ChoosenDbProviderFactory.GetInstance();

            StockContext testContext = null;
            Repository<Company> tested = null;
            try
            {
                testContext = new StockContext(options);
                testContext.Database.EnsureCreated();
                tested = new Repository<Company>(testContext);
                tested.Add(company);
                var changesCount = testContext.SaveChanges();

                var actual = tested.Entities.Count();
                var expected = 1;

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
            var options = Config.ChoosenDbProviderFactory.GetInstance();

            StockContext testContext = null;
            Repository<Company> tested = null;
            try
            {
                testContext = new StockContext(options);
                testContext.Database.EnsureCreated();
                tested = new Repository<Company>(testContext);
                tested.Add(company);
                var changesCount = testContext.SaveChanges();


                tested.Remove(company);
                var changesCount2 = testContext.SaveChanges();

                var actual = tested.Entities.Count();
                var expected = 0;

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
            var options = Config.ChoosenDbProviderFactory.GetInstance();

            StockContext testContext = null;
            Repository<Company> tested = null;
            try
            {
                testContext = new StockContext(options);
                testContext.Database.EnsureCreated();
                tested = new Repository<Company>(testContext);
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
