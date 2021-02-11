using System.Threading.Tasks;
using LoggerLite;
using Stocks.Data.Api.Models;

namespace Stocks.Data.Api.Services
{
    public interface IDatabaseManagementService
    {
        Task<bool> EnsureDbExists(ProjectSettings project, int retries = 3);
        Task<bool> DropLocalDbAsync(ProjectSettings project, int retries = 3);
    }
}