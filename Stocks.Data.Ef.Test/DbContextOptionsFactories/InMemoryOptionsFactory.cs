using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using Microsoft.EntityFrameworkCore;
using StandardInterfaces;

namespace Stocks.Data.Ef.Test.DbContextOptionsFactories
{
    public class InMemoryOptionsFactory : IFactory<DbContextOptions<StockContext>>
    {
        public DbContextOptions<StockContext> GetInstance()
        {
            return new DbContextOptionsBuilder<StockContext>()
                .UseInMemoryDatabase(Path.GetRandomFileName())
                .Options;
        }
    }
}
