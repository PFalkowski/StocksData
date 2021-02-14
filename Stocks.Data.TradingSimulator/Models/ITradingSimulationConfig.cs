using System;
using System.Text.RegularExpressions;

namespace Stocks.Data.TradingSimulator.Models
{
    public interface ITradingSimulationConfig
    {
        DateTime FromDate { get; set; }
        DateTime ToDate { get; set; }
        double StartingCash { get; set; }
        Regex BlackListPattern { get; set; }
    }
}