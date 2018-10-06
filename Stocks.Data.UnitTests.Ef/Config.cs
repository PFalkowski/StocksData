using Microsoft.EntityFrameworkCore;
using StandardInterfaces;
using Stocks.Data.UnitTests.Ef.Test.Mocks;

namespace Stocks.Data.UnitTests.Ef
{
    internal static class Config
    {
        public static IFactory<DbContextOptions<DbContext>> ChoosenDbProviderFactory { get; }
            = new InMemoryTestOptionsFactory();
            //= new LocalDbTestOptionsFactory();
    }
}
