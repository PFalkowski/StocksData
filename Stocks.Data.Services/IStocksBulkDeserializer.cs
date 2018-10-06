using System.Collections.Generic;
using Stocks.Data.Model;

namespace Stocks.Data.Services
{
    public interface IStocksBulkDeserializer
    {
        List<Company> Deserialize(IDictionary<string, string> files);
    }
}