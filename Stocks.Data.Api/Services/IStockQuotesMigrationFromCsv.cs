using System.Threading.Tasks;
using Stocks.Data.Common.Models;

namespace Stocks.Data.Api.Services
{
    public interface IStockQuotesMigrationFromCsv
    {
        Task Migrate(IProjectSettings project);
        Task Migrate(IProjectSettings project, TargetLocation location);
    }
}