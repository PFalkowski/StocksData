using System.Threading.Tasks;
using Stocks.Data.Common.Models;

namespace Stocks.Data.Api.Services
{
    public interface IStockQuotesDownloadService
    {
        Task Download(IProjectSettings project);
    }
}