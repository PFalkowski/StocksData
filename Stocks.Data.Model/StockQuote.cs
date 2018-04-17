using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Globalization;
using System.Linq;
using Extensions.Standard;

namespace Stocks.Data.Model
{
    public class StockQuote : IValidatable, IValueEquatable<StockQuote>
    {
        public virtual Company Company { get; set; }

        //[Column(Order = 0), Key]
        //[Key]
        public virtual string Ticker { get; set; }

        //[Column(Order = 1), Key]
        //[Key]
        public virtual int Date { get; set; }

        public virtual double Open { get; set; }
        public virtual double High { get; set; }
        public virtual double Low { get; set; }
        public virtual double Close { get; set; }
        public virtual double Volume { get; set; }

        [NotMapped]
        public virtual DateTime DateParsed => DateTime.ParseExact(Date.ToString(), "yyyyMMdd", CultureInfo.InvariantCulture);

        public virtual bool ValueEquals(StockQuote other)
        {
            return other.Ticker == Ticker &&
                   other.Date == Date;
        }

        public virtual bool IsValid()
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

        public override bool Equals(object obj)
        {
            if (!(obj is StockQuote cast)) return false;
            return this.ValueEquals(cast);
        }

        public override int GetHashCode()
        {
            unchecked { return Date + Ticker.Select(x => int.Parse(((int)x).ToString())).Sum(); }
        }

        public override string ToString()
        {
            return $"{Ticker} {Date}";
        }
    }
}
