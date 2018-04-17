using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using Extensions.Standard;

namespace Stocks.Data.Model
{
    public class Company : IValidatable
    {
        [Key]
        //[ForeignKey(nameof(Quotes))]
        public virtual string Ticker { get; set; }
        public virtual ICollection<StockQuote> Quotes { get; set; }

        [NotMapped]
        public virtual StockQuote FirstQuote => Quotes?.First();
        [NotMapped]
        public virtual StockQuote LastQuote => Quotes?.Last();

        public virtual bool IsValid()
        {
            return ((string.IsNullOrWhiteSpace(Ticker)) || Quotes != null) && Quotes.All(q => q.IsValid());
        }

        public override string ToString()
        {
            return Ticker;
        }
    }
}
