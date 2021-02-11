using System;
using System.ComponentModel.DataAnnotations.Schema;
using System.Globalization;
using StandardInterfaces;

namespace Stocks.Data.Model
{
    public class StockQuote : IValidatable, IValueEquatable<StockQuote>
    {
        public Company Company { get; set; }

        public string Ticker { get; set; }

        public int Date { get; set; }

        public double Open { get; set; }
        public double High { get; set; }
        public double Low { get; set; }
        public double Close { get; set; }
        public double Volume { get; set; }
        public long? TotalSharesEmitted { get; set; }
        public double? MarketCap { get; set; }
        public double? BookValue { get; set; }
        public double? DividendYield { get; set; }
        public double? PriceToEarningsRatio { get; set; }

        [NotMapped]
        public double? PriceToBookValueRatio => MarketCap / BookValue;
        [NotMapped]
        public double? LastYearYield => MarketCap / PriceToEarningsRatio;
        [NotMapped]
        public double? BookValuePerShare => BookValue / TotalSharesEmitted;
        [NotMapped]
        public double? EarningsPerShare => LastYearYield / TotalSharesEmitted;

        [NotMapped]
        public DateTime DateParsed => DateTime.ParseExact(Date.ToString(), "yyyyMMdd", CultureInfo.InvariantCulture);

        public bool ValueEquals(StockQuote other)
        {
            return other.Ticker == Ticker &&
                   other.Date == Date;
        }

        public bool IsValid()
        {
            return Open > 0 &&
                   High > 0 &&
                   Low > 0 &&
                   Close > 0 &&
                   High >= Low &&
                   Open >= Low &&
                   Close >= Low &&
                   Open <= High &&
                   Close <= High;
        }

        public override string ToString()
        {
            return $"{Ticker} {Date}";
        }
    }
}
