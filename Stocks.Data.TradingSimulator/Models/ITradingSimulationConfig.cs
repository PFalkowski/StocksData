using System;

namespace Stocks.Data.TradingSimulator.Models
{
    public interface ITradingSimulationConfig
    {
        DateTime FromDate { get; set; }
        DateTime ToDate { get; set; }
        double StartingCash { get; set; }
        int TopN { get; set; }
    }
}