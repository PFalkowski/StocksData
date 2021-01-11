using System.Threading.Tasks;
using Stocks.Data.Api.Models;

namespace Stocks.Data.Api.Services
{
    public interface IStockQuotesDownloadService
    {
        Task Download(Project project);
    }
}