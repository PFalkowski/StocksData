using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.EntityFrameworkCore;
using Stocks.Data.Ef.Test.Mocks;
using Stocks.Data.Model;
using Xunit;

namespace Stocks.Data.Ef.Test
{
    public class StockUnitOfWorkFactoryTest
    {
        [Fact]
        public void FactoryDisposes()
        {
            // Arrange

            var options = Config.ChoosenDbProviderFactory.GetInstance();

            var testContext = new StockContext(options);
            var ufo = new StockUnitOfWorkFactory(testContext);
            var uow = ufo.GetInstance();

            ufo.Dispose();
            Assert.Throws<ObjectDisposedException>(() => ufo.GetInstance());
            Assert.Throws<ObjectDisposedException>(() => uow.Complete());
            Assert.Throws<ObjectDisposedException>(() => testContext.Companies.Find(1));
        }
    }
}
