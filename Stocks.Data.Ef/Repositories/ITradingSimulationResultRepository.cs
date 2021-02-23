using System;
using StandardInterfaces;
using Stocks.Data.Model;

namespace Stocks.Data.Ef.Repositories
{
    public interface ITradingSimulationResultRepository : IRepository<TradingSimulationResult>, IDisposable
    {
    }
}