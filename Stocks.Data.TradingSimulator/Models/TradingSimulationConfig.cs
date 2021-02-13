using System;
using System.Collections.Generic;
using System.Text;

namespace Stocks.Data.TradingSimulator.Models
{
    public class TradingSimulationConfig
    {
        public DateTime FromDate { get; set; }
        public   DateTime ToDate { get; set; }
        public double StartingCash { get; set; }
        public string BlackListPattern { get; set; } = @".*\d{3,}|WIG.*|RC.*|INTL.*|INTS.*|WIG.*|.*PP\d.*";
    }
}
