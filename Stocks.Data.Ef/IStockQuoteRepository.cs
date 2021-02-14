using StandardInterfaces;
using Stocks.Data.Model;

namespace Stocks.Data.Ef
{
    public interface IStockQuoteRepository : IRepository<StockQuote>
    {
    }
}