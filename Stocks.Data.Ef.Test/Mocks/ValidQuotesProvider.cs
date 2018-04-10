using System;
using System.Collections.Generic;
using System.Text;
using Xunit;

namespace Stocks.Data.Model.Test.Mocks
{
    public class ValidQuotesProvider : TheoryData<StockQuote>
    {
        public const string TestTicker = "TestCompany";
        public ValidQuotesProvider()
        {
            Add(new StockQuote
            {
                Open = 10,
                High = 12,
                Low = 9,
                Close = 10,
                Volume = 100,
                Ticker = TestTicker,
                Date = 19900101
            });
            Add(new StockQuote
            {
                Open = 11.67,
                High = 12.9,
                Low = 11,
                Close = 11.2,
                Volume = 100,
                Ticker = TestTicker,
                Date = 19900102
            });
        }
    }
}
