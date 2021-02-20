using System;
using System.Collections.Generic;
using ProgressReporting;
using Stocks.Data.Model;
using Stocks.Data.TradingSimulator.Models;

namespace Stocks.Data.TradingSimulator
{
    public interface ITradingSimulator
    {
        SimulationResult Simulate(List<StockQuote> allQuotesPrefilterd, TradingSimulationConfig tradingSimulationConfig, IProgressReportable progress = null);
        List<StockQuote> GetSignals(TradingSimulationConfig tradingSimulationConfig, DateTime date);
    }
}