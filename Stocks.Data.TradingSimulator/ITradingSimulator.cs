using System.Collections.Generic;
using ProgressReporting;
using Stocks.Data.Model;
using Stocks.Data.TradingSimulator.Models;

namespace Stocks.Data.TradingSimulator
{
    public interface ITradingSimulator
    {
        SimulationResult Simulate(List<Company> companies, TradingSimulationConfig tradingSimulationConfig, IProgressReportable progress = null);
    }
}