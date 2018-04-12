using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using Microsoft.EntityFrameworkCore;
using StandardInterfaces;

namespace Stocks.Data.Ef.Test.DbContextOptionsFactories
{
    public class InMemoryOptionsFactory : IFactory<DbContextOptions<DbContext>>
    {
        public DbContextOptions<DbContext> GetInstance()
        {
            return new DbContextOptionsBuilder<DbContext>()
                .UseInMemoryDatabase(Path.GetFileNameWithoutExtension(Path.GetRandomFileName()))
                .Options;
        }
    }
}
