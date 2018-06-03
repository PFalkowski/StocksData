using Stocks.Data.Model;
using Xunit;

namespace Stocks.Data.UnitTests.Model.Mocks
{
    public class InvalidQuotesProvider : TheoryData<StockQuote>
    {
        public const string TestTicker = "TestCompany";
        public InvalidQuotesProvider()
        {
            Add(
                new StockQuote
                {
                    Open = 14,
                    High = 12,
                    Low = 9,
                    Close = 10,
                    Volume = 100,
                    Ticker = TestTicker,
                    Date = 19900101
                });
        }
    }
}
