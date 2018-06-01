using Stocks.Data.Model;
using Xunit;

namespace Stocks.Data.UnitTests.Model.Mocks
{
    public class EqualQuotePairsProvider : TheoryData<StockQuote, StockQuote>
    {
        public EqualQuotePairsProvider()
        {
            Add(new StockQuote { Ticker = "IDK", Date = 19900101 }, new StockQuote { Ticker = "IDK", Date = 19900101 });
            Add(new StockQuote { Ticker = "MBK", Date = 20110730 }, new StockQuote { Ticker = "MBK", Date = 20110730 });
            Add(new StockQuote { Ticker = "KGM", Date = 20150615 }, new StockQuote { Ticker = "KGM", Date = 20150615 });
            Add(new StockQuote { Ticker = "ZZZ", Date = 99991231 }, new StockQuote { Ticker = "ZZZ", Date = 99991231 });
            Add(new StockQuote { Ticker = "AAA", Date = 00000101 }, new StockQuote { Ticker = "AAA", Date = 00000101 });
        }
    }
}
