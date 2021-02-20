using Extensions.Standard;
using Microsoft.Extensions.Configuration;
using System;
using System.Globalization;
using System.Linq;

namespace Stocks.Data.TradingSimulator.Models
{
    public class TradingSimulationConfig
    {
        public DateTime FromDate { get; set; }
        public DateTime ToDate { get; set; }
        public double StartingCash { get; set; }
        public int TopN { get; set; }
        public double LowerPriceChangeFilter { get; set; }
        public double UpperPriceChangeFilter { get; set; }

        public static TradingSimulationConfig CreateFromConfig(IConfiguration configuration)
        {
            return new TradingSimulationConfig
            {
                FromDate = DateTime.Parse(configuration[nameof(FromDate)], CultureInfo.InvariantCulture),
                ToDate = DateTime.Parse(configuration[nameof(ToDate)], CultureInfo.InvariantCulture),
                StartingCash = double.Parse(configuration[nameof(StartingCash)], CultureInfo.InvariantCulture),
                TopN = int.Parse(configuration[nameof(TopN)]),
                LowerPriceChangeFilter = double.Parse(configuration[nameof(LowerPriceChangeFilter)], CultureInfo.InvariantCulture),
                UpperPriceChangeFilter = double.Parse(configuration[nameof(UpperPriceChangeFilter)], CultureInfo.InvariantCulture)
            };
        }

        public override string ToString()
        {
            var settings = this.GetAllPublicPropertiesValues();

            return string.Join(", ", settings.Select(x => $"{x.Key} = {x.Value}"));
        }
    }
}