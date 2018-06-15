using System.IO;
using Microsoft.EntityFrameworkCore;
using StandardInterfaces;

namespace Stocks.Data.UnitTests.Ef.Test.DbContextOptionsFactories
{
    public class LocalDbTestOptionsFactory : IFactory<DbContextOptions<DbContext>>
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
