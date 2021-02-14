using System;
using System.Text.RegularExpressions;

namespace Stocks.Data.TradingSimulator.Models
{
    public class TradingSimulationConfig : ITradingSimulationConfig
    {
        public DateTime FromDate { get; set; }
        public DateTime ToDate { get; set; }
        public double StartingCash { get; set; } = 1000;

        private const string PatternString = @".*\d{3,}|WIG.*|RC.*|INTL.*|INTS.*|WIG.*|.*PP\d.*";
        public Regex BlackListPattern { get; set; } = new Regex(PatternString, RegexOptions.Compiled);
    }
}
