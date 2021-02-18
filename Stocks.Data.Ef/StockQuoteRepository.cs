using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Extensions.Standard;
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

        public List<DateTime> GetTradingDates(DateTime fromInclusive, DateTime toInclusive)
        {
            return Entities.Where(x => x.DateParsed >= fromInclusive && x.DateParsed <= toInclusive)
                .Select(x => x.DateParsed)
                .Distinct()
                .OrderBy(x => x)
                .ToList();
        }

        public List<DateTime> GetNTradingDatesBefore(DateTime currentDate, int n)
        {
            return Entities.Where(x => x.DateParsed < currentDate)
                .Select(x => x.DateParsed)
                .Distinct()
                .OrderByDescending(x => x)
                .Take(n)
                .ToList();
        }

        public List<StockQuote> GetAllQuotesFromPreviousNDays(DateTime currentDate, int n)
        {
            var lastDates = GetNTradingDatesBefore(currentDate, n);
            return Entities.Where(x => x.DateParsed >= lastDates.First() && x.DateParsed <= lastDates.Last())
                .ToList();
        }

        public List<StockQuote> GetAllQuotesFromPreviousSession(DateTime currentDate)
        {
            var lastSessionDates = GetNTradingDatesBefore(currentDate, 1);
            return Entities.Where(x => x.DateParsed.Equals(lastSessionDates.Single()))
                .ToList();
        }
    }
}
