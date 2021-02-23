using Stocks.Data.Model;
using System;

namespace Stocks.Data.Ef.DataTransferObjects
{
    public class CompanySummaryDto
    {
        public string Ticker { get; set; }
        public int QuotesCount { get; set; }
        public StockQuote FirstQuote { get; set; }
        public StockQuote LastQuote { get; set; }

        public override string ToString()
        {
            var nl = Environment.NewLine;
            return $"{nl}{Ticker}: {QuotesCount} quotes {nl}" +
                   $"{string.Join(nl, FirstQuote)}" +
                   $"{nl}...{nl}" +
                   $"{string.Join(nl, LastQuote)}" +
                   $"{nl}";
        }
    }
}
