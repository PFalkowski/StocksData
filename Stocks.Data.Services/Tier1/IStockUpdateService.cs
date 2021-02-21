using System;
using System.Threading.Tasks;

namespace Stocks.Data.Services.Tier1
{
    public interface IStockUpdateService
    {
        Task PerformUpdateTillToday();
    }
}