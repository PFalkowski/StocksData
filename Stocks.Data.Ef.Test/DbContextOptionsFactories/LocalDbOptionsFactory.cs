using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using Microsoft.EntityFrameworkCore;
using StandardInterfaces;

namespace Stocks.Data.Ef.Test.DbContextOptionsFactories
{
    public class LocalDbOptionsFactory : IFactory<DbContextOptions<StockContext>>
    {
        public DbContextOptions<StockContext> GetInstance()
        {
            string connectionStr = $"server=(localdb)\\MSSQLLocalDB;Initial Catalog={Path.GetRandomFileName()};Integrated Security=True;";
            return new DbContextOptionsBuilder<StockContext>()
                .UseSqlServer(connectionStr)
                .Options;
        }
    }
}
