using System;
using System.Collections.Generic;
using Services.IO;
using Stocks.Data.Model;

namespace Stocks.Data.Services
{
    public class StocksFileProvider : IStocksFileProvider
    {
        public IDirectoryService DirectorySvc { get; }
        public IStocksBulkDeserializer DeserializationService { get; }

        public StocksFileProvider(IDirectoryService directorySvc, IStocksBulkDeserializer deserializationSvc)
        {
            DirectorySvc = directorySvc;
            DeserializationService = deserializationSvc;
            ValidateState();
        }

        public List<Company> ReadStocksFrom(string directory, string pattern = "*.msc")
        {
            ValidateState();
            var stocksRaw = DirectorySvc.ReadTopDirectory(directory, pattern);

            var deserialized = DeserializationService.Deserialize(stocksRaw);
            return deserialized;
        }

        private void ValidateState()
        {
            if (DirectorySvc == null) throw new InvalidOperationException($"{nameof(DirectorySvc)} not initialized");
            if (DeserializationService == null) throw new InvalidOperationException($"{nameof(DeserializationService)} not initialized");
        }
    }
}
