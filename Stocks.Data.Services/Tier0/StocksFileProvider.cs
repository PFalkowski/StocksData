﻿using System;
using System.Collections.Generic;
using System.IO;
using Services.IO;
using Stocks.Data.Model;

namespace Stocks.Data.Services.Tier0
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

        public List<Company> ReadStocksFrom(DirectoryInfo directory, string pattern = "*.msc")
        {
            return ReadStocksFrom(directory.FullName, pattern);
        }
        public List<Company> ReadStocksFrom(string directory, string pattern = "*.msc")
        {
            ValidateState();
            var stocksRaw = DirectorySvc.ReadTopDirectory(directory, pattern);

            var deserialized = DeserializationService.DeserializeParallel(stocksRaw);
            return deserialized;
        }

        private void ValidateState()
        {
            if (DirectorySvc == null) throw new InvalidOperationException($"{nameof(DirectorySvc)} not initialized");
            if (DeserializationService == null) throw new InvalidOperationException($"{nameof(DeserializationService)} not initialized");
        }
    }
}
