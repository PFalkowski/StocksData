using System;
using Stocks.Data.Ef;
using Stocks.Data.UnitTests.Ef.Test.Config;
using Xunit;

namespace Stocks.Data.UnitTests.Ef.Test
{
    public class StockUnitOfWorkFactoryTest
    {
        [Fact]
        public void FactoryProvidesNewInstanceEverytimeItsCalled()
        {
            // Arrange
            
            var testSettings = new TestProjectSettings();
            StockContext testContext = null;
            StockUnitOfWorkFactory ufo = null;
            StockUnitOfWork uow = null;
            StockUnitOfWork uow2 = null;
            try
            {
                testContext = new StockContext(testSettings);
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
            
            var testSettings = new TestProjectSettings();

            var testContext = new StockContext(testSettings);
            var ufo = new StockUnitOfWorkFactory(testContext);
            var uow = ufo.GetInstance();

            ufo.Dispose();

            // Act & Assert

            Assert.Throws<ObjectDisposedException>(() => ufo.GetInstance());
            Assert.Throws<ObjectDisposedException>(() => uow.Complete());
            Assert.Throws<ObjectDisposedException>(() => testContext.Company.Find(1));
        }
    }
}
