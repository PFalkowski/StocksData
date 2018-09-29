using System.Collections.Generic;
using Stocks.Data.Model;

namespace Services
{
    public interface IStocksFileProvider
    {
        List<Company> ReadStocksFrom(string directory, string pattern);
    }
}