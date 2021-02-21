using System;
using System.Threading.Tasks;
using Stocks.Data.Common.Models;

namespace Stocks.Data.Services.Tier1
{
    public interface IStockQuotesDownloadService
    {
        Task Download(IProjectSettings project);
        Task<string> DownloadUpdate(IProjectSettings project, DateTime date);
    }
}