using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Stocks.Data.Model;

namespace Stocks.Data.Services
{
    public class StocksBulkDeserializer : IStocksBulkDeserializer
    {
        public IStocksDeserializer Deserializer { get; }

        public StocksBulkDeserializer(IStocksDeserializer deserializer)
        {
            Deserializer = deserializer ?? throw new ArgumentNullException(nameof(deserializer));
        }

        public List<Company> Deserialize(IDictionary<string, string> files)
        {
            var allStocks = new ConcurrentBag<Company>();

            Parallel.ForEach(files,
                delegate (KeyValuePair<string, string> file)
                {
                    allStocks.Add(Deserializer.Deserialize(file.Value));
                });

            return allStocks.ToList();
        }
    }
}
