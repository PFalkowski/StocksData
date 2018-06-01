using System;
using Microsoft.EntityFrameworkCore;
using Xunit;

namespace Stocks.Data.Ef.Test
{
    public class StockUnitOfWorkFactoryTest
    {
        [Fact]
        public void FactoryProvidesNewInstanceEverytimeItsCalled()
        {
            // Arrange

            DbContextOptions<DbContext> options = Config.ChoosenDbProviderFactory.GetInstance(); ;
            StockContext testContext = null;
            StockUnitOfWorkFactory ufo = null;
            StockUnitOfWork uow = null;
            StockUnitOfWork uow2 = null;
            try
            {
                testContext = new StockContext(options);
                ufo = new StockUnitOfWorkFactory(testContext);

                // Act

                uow = ufo.GetInstance();
                uow2 = ufo.GetInstance();

                // Assert

                Assert.NotEqual(uow, uow2);
            }
            finally
            {
                testContext?.Dispose();
                ufo?.Dispose();
                uow?.Dispose();
            }
        }
        [Fact]
        public void FactoryDisposes()
        {
            // Arrange

            var options = Config.ChoosenDbProviderFactory.GetInstance();

            var testContext = new StockContext(options);
            var ufo = new StockUnitOfWorkFactory(testContext);
            var uow = ufo.GetInstance();

            ufo.Dispose();

            // Act & Assert

            Assert.Throws<ObjectDisposedException>(() => ufo.GetInstance());
            Assert.Throws<ObjectDisposedException>(() => uow.Complete());
            Assert.Throws<ObjectDisposedException>(() => testContext.Companies.Find(1));
        }
    }
}
