using Microsoft.EntityFrameworkCore;
using Stocks.Data.Common.Models;

namespace Stocks.Data.UnitTests.Ef.Test.TestData
{
    public class MockPocoContext : DbContext
    {
        public MockPocoContext(IProjectSettings projectSettings) : base(projectSettings.GetDbContextOptions)
        {
        }
        public virtual DbSet<MockPoco> Pocos { get; set; }
    }
}
