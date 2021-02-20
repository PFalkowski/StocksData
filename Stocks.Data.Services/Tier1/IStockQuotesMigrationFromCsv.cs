using System.Threading.Tasks;
using Stocks.Data.Common.Models;

namespace Stocks.Data.Services.Tier1
{
    public interface IStockQuotesMigrationFromCsv
    {
        Task Migrate(IProjectSettings project, TargetLocation location);
        Task Unzip(IProjectSettings project);
    }
}