using System;
using System.Collections.Generic;
using System.Text;
using Xunit;

namespace Stocks.Data.Model.Test.Mocks
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
