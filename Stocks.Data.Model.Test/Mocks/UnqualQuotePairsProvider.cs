using System;
using System.Collections.Generic;
using System.Text;
using Xunit;

namespace Stocks.Data.Model.Test.Mocks
{
    public class UnqualQuotePairsProvider : TheoryData<StockQuote, StockQuote>
    {
        public UnqualQuotePairsProvider()
        {
            Add(new StockQuote { Ticker = "IDK", Date = 19900101 }, new StockQuote { Ticker = "IDK", Date = 19900102 });
            Add(new StockQuote { Ticker = "IDK", Date = 19900101 }, new StockQuote { Ticker = "IDL", Date = 19900101 });
            Add(new StockQuote { Ticker = "\\DK", Date = 19900101 }, new StockQuote { Ticker = "IDK", Date = 19900101 });
        }
    }
}
