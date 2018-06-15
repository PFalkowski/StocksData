using System.IO;
using Microsoft.EntityFrameworkCore;
using StandardInterfaces;

namespace Stocks.Data.UnitTests.Ef.Test.DbContextOptionsFactories
{
    public class InMemoryTestOptionsFactory : IFactory<DbContextOptions<DbContext>>
    {
        public DbContextOptions<DbContext> GetInstance()
        {
            return new DbContextOptionsBuilder<DbContext>()
                .UseInMemoryDatabase(Path.GetFileNameWithoutExtension(Path.GetRandomFileName()))
                .Options;
        }
    }
}
