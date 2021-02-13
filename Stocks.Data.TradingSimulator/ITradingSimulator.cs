using System.Collections.Generic;
using Stocks.Data.Model;
using Stocks.Data.TradingSimulator.Models;

namespace Stocks.Data.TradingSimulator
{
    public interface ITradingSimulator
    {
        (List<StockTransaction> transactionsLedger, double finalBalance) Simulate(List<Company> companies, TradingSimulationConfig tradingSimulationConfig);
    }
}