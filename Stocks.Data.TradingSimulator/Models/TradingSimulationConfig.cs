using System;
using System.Text.RegularExpressions;
using Stocks.Data.Infrastructure;

namespace Stocks.Data.TradingSimulator.Models
{
    public class TradingSimulationConfig : ITradingSimulationConfig
    {
        public DateTime FromDate { get; set; }
        public DateTime ToDate { get; set; }
        public double StartingCash { get; set; } = 1000;
        public int TopN { get; set; } = 10;
    }
}
