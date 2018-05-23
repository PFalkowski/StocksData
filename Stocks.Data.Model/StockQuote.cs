using System;
using System.ComponentModel.DataAnnotations.Schema;
using System.Globalization;
using StandardInterfaces;

namespace Stocks.Data.Model
{
    public sealed class StockQuote : IValidatable, IValueEquatable<StockQuote>
    {
        public Company Company { get; set; }

        //[Column(Order = 0), Key]
        //[Key]
        public string Ticker { get; set; }

        //[Column(Order = 1), Key]
        //[Key]
        public int Date { get; set; }

        public double Open { get; set; }
        public double High { get; set; }
        public double Low { get; set; }
        public double Close { get; set; }
        public double Volume { get; set; }

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
