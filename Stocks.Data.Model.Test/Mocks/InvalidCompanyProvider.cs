using System.Collections.Generic;
using Xunit;

namespace Stocks.Data.Model.Test.Mocks
{
    public class InvalidCompanyProvider : TheoryData<Company>
    {
        public const string TestTicker = "TestCompany";
        public InvalidCompanyProvider()
        {
            Add(
            new Company
            {
                Ticker = TestTicker,
                Quotes = new List<StockQuote>
                    {
                            new StockQuote
                            {
                                Open = 14,
                                High = 12,
                                Low = 9,
                                Close = 10,
                                Volume = 100,
                                Ticker = TestTicker,
                                Date = 19900101
                            }
                    }
            });
        }
    }
}
