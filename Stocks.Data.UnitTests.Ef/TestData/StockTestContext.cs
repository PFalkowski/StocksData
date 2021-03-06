﻿using Microsoft.EntityFrameworkCore;
using Stocks.Data.Common.Models;
using Stocks.Data.Model;

namespace Stocks.Data.UnitTests.Ef.Test.TestData
{
    public class StockTestContext : DbContext
    {
        public StockTestContext(IProjectSettings projectSettings) : base(projectSettings.GetDbContextOptions)
        {
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<StockQuote>()
                .HasKey(q => new { q.Ticker, q.Date });
            modelBuilder.Entity<Company>()
                .HasMany(c => c.Quotes)
                .WithOne(c => c.Company)
                .HasForeignKey(q => q.Ticker)
                .HasPrincipalKey(q => q.Ticker);
        }
        public virtual DbSet<Company> Company { get; set; }
    }
}
