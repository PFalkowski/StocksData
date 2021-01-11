using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using StandardInterfaces;

namespace Stocks.Data.Model
{
    public sealed class Company : IValidatable
    {
        [Key]
        public string Ticker { get; set; }
        public string ISIN { get; set; }
        public string Sector { get; set; }
        public List<StockQuote> Quotes { get; set; }

        [NotMapped]
        public StockQuote FirstQuote => Quotes?.First();
        [NotMapped]
        public StockQuote LastQuote => Quotes?.Last();

        public bool IsValid()
        {
            return (string.IsNullOrWhiteSpace(Ticker) || Quotes != null) && Quotes.All(q => q.IsValid());
        }

        public override string ToString()
        {
            return Ticker;
        }
    }
}
