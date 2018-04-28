using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using Microsoft.EntityFrameworkCore;
using StandardInterfaces;
using Stocks.Data.Infrastructure;
using Stocks.Data.Model;

namespace Stocks.Data.Ef
{
    public sealed class StockUnitOfWork : IUnitOfWork
    {
        private DbContext Context { get; }
        public Repository<Company> StockRepository { get; set; }

        public StockUnitOfWork(DbContext context)
        {
            var casted = (StockContext)context;
            Context = casted;
            StockRepository = new Repository<Company>(casted);
        }

        public void Complete()
        {
            Context?.SaveChanges();
        }

        public void Dispose()
        {
            Context?.Dispose();
        }
    }
}
