using System.Collections.Generic;
using System.Threading.Tasks;
using Stocks.Data.Model;

namespace Stocks.Data.Services.Tier0
{
    public interface IStocksBulkDeserializer
    {
        List<Company> DeserializeParallel(IDictionary<string, string> files);
        Task<List<Company>> DeserializeAsync(IDictionary<string, string> files);
        List<StockQuote> Deserialize(string quotes);
    }
}