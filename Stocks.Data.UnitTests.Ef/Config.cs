using Microsoft.EntityFrameworkCore;
using StandardInterfaces;
using Stocks.Data.UnitTests.Ef.Test.DbContextOptionsFactories;

namespace Stocks.Data.UnitTests.Ef.Test
{
    internal static class Config
    {
        public static IFactory<DbContextOptions<DbContext>> ChoosenDbProviderFactory { get; }
            = new InMemoryTestOptionsFactory();
            //= new LocalDbOptionsFactory();
    }
}
