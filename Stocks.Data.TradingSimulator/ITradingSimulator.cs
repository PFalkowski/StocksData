using System;
using System.Collections.Generic;
using ProgressReporting;
using Stocks.Data.Model;
using Stocks.Data.TradingSimulator.Models;

namespace Stocks.Data.TradingSimulator
{
    public interface ITradingSimulator
    {
        SimulationResult Simulate(TradingSimulationConfig tradingSimulationConfig, IProgressReportable progress = null);
        List<StockQuote> GetTopN(List<StockQuote> allQuotesPrefilterd, DateTime date);
    }
}