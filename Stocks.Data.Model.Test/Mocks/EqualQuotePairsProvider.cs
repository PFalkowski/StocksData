using System;
using System.Collections.Generic;
using System.Text;
using Xunit;

namespace Stocks.Data.Model.Test.Mocks
{
    public class EqualQuotePairsProvider : TheoryData<StockQuote, StockQuote>
    {
        public EqualQuotePairsProvider()
        {
            Add(new StockQuote { Ticker = "IDK", Date = 19900101 }, new StockQuote { Ticker = "IDK", Date = 19900101 });
        }
    }
}
