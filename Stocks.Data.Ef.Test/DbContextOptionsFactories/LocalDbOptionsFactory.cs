using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using Microsoft.EntityFrameworkCore;
using StandardInterfaces;

namespace Stocks.Data.Ef.Test.DbContextOptionsFactories
{
    public class LocalDbOptionsFactory : IFactory<DbContextOptions<DbContext>>
    {
        public DbContextOptions<DbContext> GetInstance()
        {
            string connectionStr = $"server=(localdb)\\MSSQLLocalDB;Initial Catalog={Path.GetFileNameWithoutExtension(Path.GetRandomFileName())};Integrated Security=True;";
            return new DbContextOptionsBuilder<DbContext>()
                .UseSqlServer(connectionStr)
                .Options;
        }
    }
}
