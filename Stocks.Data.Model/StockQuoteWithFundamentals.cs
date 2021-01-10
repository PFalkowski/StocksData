using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace Stocks.Data.Model
{
    public class StockQuoteWithFundamentals : StockQuote
    {
        public long TotalSharesEmitted { get; set; }
        public double MarketCap { get; set; }
        public double BookValue { get; set; }
        public double? DividendYield { get; set; }
        public double PriceToEarningsRatio { get; set; }

        [NotMapped]
        public double PriceToBookValueRatio => MarketCap / BookValue;
        [NotMapped]
        public double LastYearYield => MarketCap / PriceToEarningsRatio;
        [NotMapped]
        public double BookValuePerShare => BookValue / TotalSharesEmitted;
        [NotMapped]
        public double EarningsPerShare => LastYearYield / TotalSharesEmitted;
    }
}
