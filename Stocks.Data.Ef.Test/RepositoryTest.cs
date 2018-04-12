using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using Stocks.Data.Ef.Test.Mocks;
using Stocks.Data.Model;
using Xunit;

namespace Stocks.Data.Ef.Test
{
    public class RepositoryTest
    {
        [Theory]
        [ClassData(typeof(MockPocoRangeProvider))]
        public void AddRangeAddsRange(IEnumerable<MockPoco> input)
        {
            var options = Config.ChoosenDbProviderFactory.GetInstance();

            DbContext testContext = null;
            Repository<MockPoco> tested = null;
            try
            {
                testContext = new MockPocoContext(options);
                tested = new Repository<MockPoco>(testContext);
                testContext.Database.EnsureCreated();

                tested.AddRange(input);
                testContext.SaveChanges();

                Assert.Equal(input.Count(), tested.Entities.Count());
            }
            finally
            {
                testContext?.Database.EnsureDeleted();
                tested?.Dispose();
                testContext?.Dispose();
            }
        }
        [Theory]
        [ClassData(typeof(MockPocoRangeProvider))]
        public void RemoveRangeRemovesRange(IEnumerable<MockPoco> input)
        {
            var options = Config.ChoosenDbProviderFactory.GetInstance();

            DbContext testContext = null;
            Repository<MockPoco> tested = null;
            try
            {
                testContext = new MockPocoContext(options);
                tested = new Repository<MockPoco>(testContext);
                testContext.Database.EnsureCreated();

                tested.AddRange(input);
                testContext.SaveChanges();

                Assert.Equal(input.Count(), tested.Entities.Count());

                tested.RemoveRange(input);
                testContext.SaveChanges();
                Assert.Empty(tested.Entities);
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
