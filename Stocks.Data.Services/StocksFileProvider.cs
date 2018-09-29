using Services.IO;
using Stocks.Data.Infrastructure;
using Stocks.Data.Model;
using System;
using System.Collections.Generic;

namespace Services
{
    public class StocksFileProvider : IStocksFileProvider
    {
        public IDirectoryService DirectorySvc { get; set; }
        public IStocksBulkDeserializer DeserializationService { get; set; }

        public StocksFileProvider(IDirectoryService directorySvc, IStocksBulkDeserializer deserializationSvc)
        {
            DirectorySvc = directorySvc;
            DeserializationService = deserializationSvc;
        }

        public List<Company> ReadStocksFrom(string directory, string pattern = "*.msc")
        {
            var directorySvc = DirectorySvc ?? new DirectoryService(new FileService());
            var deserializationSvc = DeserializationService ?? new StocksBulkDeserializer(new StocksDeserializer(new StockQuoteCsvClassMap()));
            var stocksRaw = directorySvc.ReadTopDirectory(directory, pattern);

            var deserialized = deserializationSvc.Deserialize(stocksRaw);
            return deserialized;
        }
    }
}
