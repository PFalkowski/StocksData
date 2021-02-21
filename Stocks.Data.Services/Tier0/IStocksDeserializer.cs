using System.Collections.Generic;
using System.Threading.Tasks;
using Stocks.Data.Model;

namespace Stocks.Data.Services.Tier0
{
    public interface IStocksDeserializer
    {
        Company Deserialize(string fileContents);
        Task<Company> DeserializeAsync(string fileContents);
        List<StockQuote> DeserializeQuotes(string fileContents);
    }
}