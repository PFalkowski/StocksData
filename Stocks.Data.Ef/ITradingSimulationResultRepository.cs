using System;
using StandardInterfaces;
using Stocks.Data.Model;

namespace Stocks.Data.Ef
{
    public interface ITradingSimulationResultRepository : IRepository<TradingSimulationResult>, IDisposable
    {
    }
}