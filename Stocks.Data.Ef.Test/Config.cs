using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.EntityFrameworkCore;
using StandardInterfaces;
using Stocks.Data.Ef.Test.DbContextOptionsFactories;

namespace Stocks.Data.Ef.Test
{
    public static class Config
    {
        public static IFactory<DbContextOptions<DbContext>> ChoosenDbProviderFactory { get; }
            = new InMemoryOptionsFactory();
            //= new LocalDbOptionsFactory();
    }
}
