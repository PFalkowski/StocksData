using System.Collections.Generic;
using System.IO;
using Stocks.Data.Model;

namespace Stocks.Data.Services
{
    public interface IStocksFileProvider
    {
        List<Company> ReadStocksFrom(string directory, string pattern);
        List<Company> ReadStocksFrom(DirectoryInfo directory, string pattern);
    }
}