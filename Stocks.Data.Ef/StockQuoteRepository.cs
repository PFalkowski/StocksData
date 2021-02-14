using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.EntityFrameworkCore;
using StandardInterfaces;
using Stocks.Data.Model;

namespace Stocks.Data.Ef
{
    public class StockQuoteRepository : Repository<StockQuote>, IStockQuoteRepository
    {
        public StockQuoteRepository(DbContext context) : base(context)
        {
        }
    }
}
