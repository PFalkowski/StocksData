﻿using Stocks.Data.Model;
using Xunit;

namespace Stocks.Data.UnitTests.Model.TestData
{
    public class ValidStockQuoteWithFundamentalsProvider : TheoryData<StockQuoteWithFundamentals>
    {
        public const string TestTicker = "TestCompany";
        public ValidStockQuoteWithFundamentalsProvider()
        {
            Add(new StockQuoteWithFundamentals
            {
                Open = 10,
                High = 12,
                Low = 9,
                Close = 10,
                Volume = 100,
                Ticker = TestTicker,
                Date = 19900101,
                BookValue = 802252668.14,
                MarketCap = 906596831,
                TotalSharesEmitted = 7775273,
                PriceToEarningsRatio = 8.398,
                DividendYield = 2.57
            });
        }
    }
}
