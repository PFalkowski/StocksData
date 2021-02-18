using System;
using System.Collections.Generic;
using ProgressReporting;
using Stocks.Data.Model;
using Stocks.Data.TradingSimulator.Models;

namespace Stocks.Data.TradingSimulator
{
    public interface ITradingSimulator
    {
        SimulationResult Simulate(ITradingSimulationConfig tradingSimulationConfig, IProgressReportable progress = null);
        List<StockQuote> GetSignals(ITradingSimulationConfig tradingSimulationConfig, DateTime date);
    }
}