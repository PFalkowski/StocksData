using System;
using System.Collections.Generic;
using StandardInterfaces;
using Stocks.Data.Model;

namespace Stocks.Data.Ef
{
    public interface IStockQuoteRepository : IRepository<StockQuote>, IDisposable
    {
        List<DateTime> GetTradingDates(DateTime fromInclusive, DateTime toInclusive);
        List<DateTime> GetNTradingDatesBefore(DateTime currentDate, int n);
        List<StockQuote> GetAllQuotesFromPreviousNDays(DateTime currentDate, int n);
        List<StockQuote> GetAllQuotesFromPreviousSession(DateTime currentDate);
    }
}