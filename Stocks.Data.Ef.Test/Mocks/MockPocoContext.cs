using Microsoft.EntityFrameworkCore;

namespace Stocks.Data.UnitTests.Ef.Test.Mocks
{
    public class MockPocoContext : DbContext
    {
        public MockPocoContext(DbContextOptions<DbContext> options) : base(options)
        {
        }
        public virtual DbSet<MockPoco> Pocos { get; set; }
    }
}
