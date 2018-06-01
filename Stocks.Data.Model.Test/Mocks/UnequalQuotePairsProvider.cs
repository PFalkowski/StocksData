using Xunit;

namespace Stocks.Data.Model.Test.Mocks
{
    public class UnequalQuotePairsProvider : TheoryData<StockQuote, StockQuote>
    {
        public UnequalQuotePairsProvider()
        {
            Add(new StockQuote { Ticker = "IDK", Date = 19900101 }, new StockQuote { Ticker = "IDK", Date = 19900102 });
            Add(new StockQuote { Ticker = "IDK", Date = 19900101 }, new StockQuote { Ticker = "IDL", Date = 19900101 });
            Add(new StockQuote { Ticker = "\\DK", Date = 19900101 }, new StockQuote { Ticker = "IDK", Date = 19900101 });
        }
    }
}
