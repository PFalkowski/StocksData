using System;
using System.Collections.Generic;
using System.Linq;

namespace Stocks.Data.TradingSimulator.Models
{
    public class SimulationResult
    {
        public List<StockTransaction> TransactionsLedger { get; set; }
        public double FinalBalance { get; set; }
        public TradingSimulationConfig TradingSimulationConfig { get; set; }

        public double ReturnOnInvestment => (FinalBalance - TradingSimulationConfig.StartingCash) /
            TradingSimulationConfig.StartingCash * 100;

        public override string ToString()
        {
            return
                $"Simulation for {TradingSimulationConfig.FromDate.ToShortDateString()} - {TradingSimulationConfig.ToDate.ToShortDateString()} finished. Initial investment = {TradingSimulationConfig.StartingCash}. " +
                $"Final balance = {Math.Round(FinalBalance, 2)}. " +
                $"ROI = {Math.Round(ReturnOnInvestment, 2)} %. " +
                $"Total buy orders = {TransactionsLedger.Count(x => x.TransactionType == StockTransactionType.Buy)}. " +
                $"Total sell orders = {TransactionsLedger.Count(x => x.TransactionType == StockTransactionType.Sell)}";
        }
    }
}
