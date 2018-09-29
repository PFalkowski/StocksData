using System.Collections.Generic;
using Stocks.Data.Model;

namespace Services
{
    public interface IStocksBulkDeserializer
    {
        List<Company> Deserialize(IDictionary<string, string> files);
    }
}