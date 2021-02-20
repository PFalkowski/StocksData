using System;

namespace Stocks.Data.Model
{
    public class TradingSimulationResult
    {
        public int Id { get; set; }
        public DateTime SimulationDate { get; set; }
        public double InitialInvestment { get; set; }
        public double FinalBalance { get; set; }
        public double ReturnOnInvestment { get; set; }
        public int TotalBuyOrders { get; set; }
        public int TotalSellOrders { get; set; }
        public int TruePositives { get; set; }
        public int TrueNegatives { get; set; }
        public int FalsePositives { get; set; }
        public int FalseNegatives { get; set; }
        public DateTime FromDateInclusive { get; set; }
        public DateTime ToDateInclusive { get; set; }
        public int TopN { get; set; }
    }
}
