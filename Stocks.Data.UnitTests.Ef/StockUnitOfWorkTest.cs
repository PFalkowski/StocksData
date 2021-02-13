using System.Linq;
using Microsoft.EntityFrameworkCore;
using Stocks.Data.UnitTests.Ef.Test.TestData;
using Stocks.Data.Model;
using Xunit;
using Stocks.Data.Ef;
using Stocks.Data.UnitTests.Ef.Test.Config;

namespace Stocks.Data.UnitTests.Ef.Test
{
    public class StockUnitOfWorkTest
    {
        [Theory]
        [ClassData(typeof(_11BitMock))]
        public void UnitOfWorkCommits(Company input)
        {
            // Arrange

            var testSettings = new TestProjectSettings();

            DbContext testContext = null;
            StockUnitOfWork tested = null;
            try
            {
                testContext = new StockContext(testSettings);
                testContext.Database.EnsureCreated();
                tested = new StockUnitOfWork(testContext);

                // Act

                tested.StockRepository.Add(input);
                tested.Complete();

                var expected = 1;
                var actual = tested.StockRepository.Count();

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
        public void UnitOfWorkDoNotShowAsExisitngWithoutCommit(Company input)
        {
            // Arrange

            var testSettings = new TestProjectSettings();

            DbContext testContext = null;
            StockUnitOfWork tested = null;
            try
            {
                testContext = new StockContext(testSettings);
                testContext.Database.EnsureCreated();
                tested = new StockUnitOfWork(testContext);

                // Act

                tested.StockRepository.Add(input);

                var expected = 0;
                var actual = tested.StockRepository.GetAll().ToList().Count;

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
    }
}
