using System;

namespace Stocks.Data.TradingSimulator.Models
{
    public class TradingSimulationConfig : ITradingSimulationConfig
    {
        public DateTime FromDate { get; set; } = new DateTime(2000, 1, 1);
        public DateTime ToDate { get; set; } = DateTime.Now.Date;
        public double StartingCash { get; set; } = 1000;
        public int TopN { get; set; } = 10;

        public override string ToString()
        {
            return
                $"{nameof(FromDate)} = {FromDate}, {nameof(ToDate)} = {ToDate}, {nameof(StartingCash)} = {StartingCash}, {nameof(TopN)} = {TopN}";
        }
    }
}
