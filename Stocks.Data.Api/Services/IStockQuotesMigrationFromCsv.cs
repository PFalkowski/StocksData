using System.Threading.Tasks;
using Stocks.Data.Api.Models;

namespace Stocks.Data.Api.Services
{
    public interface IStockQuotesMigrationFromCsv
    {
        Task Migrate(ProjectSettings project);
        Task Migrate(ProjectSettings project, TargetLocation location);
    }
}