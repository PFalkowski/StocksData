using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.EntityFrameworkCore;
using Stocks.Data.Model;

namespace Stocks.Data.Ef.Test.Mocks
{
    public class MockPocoContext : DbContext
    {
        public MockPocoContext(DbContextOptions<DbContext> options) : base(options)
        {
        }
        public virtual DbSet<MockPoco> Pocos { get; set; }
    }
}
