using Microsoft.EntityFrameworkCore;
using StandardInterfaces;
using Stocks.Data.Ef.Test.DbContextOptionsFactories;

namespace Stocks.Data.Ef.Test
{
    internal static class Config
    {
        public static IFactory<DbContextOptions<DbContext>> ChoosenDbProviderFactory { get; }
            = new InMemoryOptionsFactory();
            //= new LocalDbOptionsFactory();
    }
}
