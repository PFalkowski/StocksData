using Stocks.Data.Model;
using System;
using System.Collections.Generic;
using Xunit;

namespace Stocks.Data.UnitTests.Model.Mocks
{
    public class ValidCompanyProvider : TheoryData<Company>
    {
        public const string TestTicker = "TestCompany";

        public static Lazy<Company> Mock => new Lazy<Company>(() =>
            new Company
            {
                Ticker = TestTicker,
                Quotes = new List<StockQuote>
                    {
                            new StockQuote
                            {
                                Open = 10,
                                High = 12,
                                Low = 9,
                                Close = 10,
                                Volume = 100,
                                Ticker = TestTicker,
                                Date = 19900101
                            },
                            new StockQuote
                            {
                                Open = 11.67,
                                High = 12.9,
                                Low = 11,
                                Close = 11.2,
                                Volume = 100,
                                Ticker = TestTicker,
                                Date = 19900102
                            }
                    }
            });

        public ValidCompanyProvider()
        {
            Add(Mock.Value);
        }
    }
}
