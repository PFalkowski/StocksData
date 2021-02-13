using System.Threading.Tasks;
using LoggerLite;
using Stocks.Data.Common.Models;

namespace Stocks.Data.Api.Services
{
    public interface IDatabaseManagementService
    {
        Task<bool> EnsureDbExists(IProjectSettings project, int retries = 3);
        Task<bool> EnsureDbDoesNotExist(IProjectSettings project, int retries = 3);
    }
}