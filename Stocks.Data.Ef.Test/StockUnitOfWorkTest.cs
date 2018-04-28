using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.EntityFrameworkCore;
using Stocks.Data.Ef.Test.Mocks;
using Stocks.Data.Model;
using Xunit;

namespace Stocks.Data.Ef.Test
{
    public class StockUnitOfWorkTest
    {
        [Theory]
        [ClassData(typeof(_11BitMock))]
        public void UnitOfWorkCommits(Company input)
        {
            // Arrange

            var options = Config.ChoosenDbProviderFactory.GetInstance();

            DbContext testContext = null;
            StockUnitOfWork tested = null;
            try
            {
                testContext = new StockContext(options);
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

            var options = Config.ChoosenDbProviderFactory.GetInstance();

            DbContext testContext = null;
            StockUnitOfWork tested = null;
            try
            {
                testContext = new StockContext(options);
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
