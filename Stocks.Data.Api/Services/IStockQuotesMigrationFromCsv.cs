using System.Threading.Tasks;
using Stocks.Data.Api.Models;

namespace Stocks.Data.Api.Services
{
    public interface IStockQuotesMigrationFromCsv
    {
        Task Migrate(Project project, TargetLocation location);
    }
}