﻿using System.Collections.Generic;
using System.Threading.Tasks;
using Stocks.Data.Model;

namespace Stocks.Data.Services
{
    public interface IStocksBulkDeserializer
    {
        List<Company> Deserialize(IDictionary<string, string> files);
        Task<List<Company>> DeserializeAsync(IDictionary<string, string> files);
    }
}