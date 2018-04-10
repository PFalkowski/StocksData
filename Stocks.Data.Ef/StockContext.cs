using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;
using Stocks.Data.Model;

namespace Stocks.Data.Ef
{
    public class StockContext : DbContext
    {
        public StockContext(DbContextOptions<StockContext> options) : base(options)
        {
        }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<StockQuote>()
                .HasKey(q => new { q.Ticker, q.Date });
            modelBuilder.Entity<StockQuote>()
                .HasOne(q => q.Company)
                .WithMany(q => q.Quotes)
                .HasForeignKey(q =>  q.Ticker )
                .HasPrincipalKey(q =>  q.Ticker );
        }
        public virtual DbSet<Company> Companies { get; set; }
    }
}
