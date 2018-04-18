using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.EntityFrameworkCore;
using StandardInterfaces;

namespace Stocks.Data.Ef
{
    public class StockUnitOfWorkFactory : IFactory<StockUnitOfWork>, IDisposable
    {
        private readonly DbContext _context;
        public StockUnitOfWorkFactory(DbContext context)
        {
            _context = context;
        }

        public StockUnitOfWork GetInstance()
        {
            return new StockUnitOfWork(_context);
        }

        public void Dispose()
        {
            _context?.Dispose();
        }
    }
}
