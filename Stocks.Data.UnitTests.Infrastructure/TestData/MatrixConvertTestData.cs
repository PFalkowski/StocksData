using System;
using System.Collections.Generic;
using Stocks.Data.Model;
using Xunit;

namespace Stocks.Data.UnitTests.Infrastructure.TestData
{
    public class MatrixConvertTestData : TheoryData<Tuple<List<StockQuote>, double[][]>>
    {
        public const string TestTicker = "TestCompany";
        public static List<StockQuote> ValidStockQuotes => new List<StockQuote>
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
            },
            new StockQuote
            {
                Open = 9.2,
                High = 9.2,
                Low = 9.2,
                Close = 9.2,
                Volume = 10,
                Ticker = TestTicker,
                Date = 19900103
            },
            new StockQuote
            {
                Open = 11.17,
                High = 12.99,
                Low = 9,
                Close = 12.2,
                Volume = 1990,
                Ticker = TestTicker,
                Date = 19900104
            },
            new StockQuote
            {
                Open = 12.27,
                High = 13.99,
                Low = 12,
                Close = 12.2,
                Volume = 19910,
                Ticker = TestTicker,
                Date = 19900105
            },
            new StockQuote
            {
                Open = 13.11,
                High = 14.39,
                Low = 12.9,
                Close = 12.98,
                Volume = 2910,
                Ticker = TestTicker,
                Date = 19900106
            },
            new StockQuote
            {
                Open = 17.11,
                High = 19.39,
                Low = 15.9,
                Close = 15.98,
                Volume = 9832,
                Ticker = TestTicker,
                Date = 19900107
            }
        };
        public static double[][] ExpectedResult => new[]
        {
            new[]{ValidStockQuotes[0].Open, ValidStockQuotes[0].High, ValidStockQuotes[0].Low, ValidStockQuotes[0].Close, ValidStockQuotes[0].Volume, },
            new[]{ValidStockQuotes[1].Open, ValidStockQuotes[1].High, ValidStockQuotes[1].Low, ValidStockQuotes[1].Close, ValidStockQuotes[1].Volume, },
            new[]{ValidStockQuotes[2].Open, ValidStockQuotes[2].High, ValidStockQuotes[2].Low, ValidStockQuotes[2].Close, ValidStockQuotes[2].Volume, },
            new[]{ValidStockQuotes[3].Open, ValidStockQuotes[3].High, ValidStockQuotes[3].Low, ValidStockQuotes[3].Close, ValidStockQuotes[3].Volume, },
            new[]{ValidStockQuotes[4].Open, ValidStockQuotes[4].High, ValidStockQuotes[4].Low, ValidStockQuotes[4].Close, ValidStockQuotes[4].Volume, },
            new[]{ValidStockQuotes[5].Open, ValidStockQuotes[5].High, ValidStockQuotes[5].Low, ValidStockQuotes[5].Close, ValidStockQuotes[5].Volume, },
            new[]{ValidStockQuotes[6].Open, ValidStockQuotes[6].High, ValidStockQuotes[6].Low, ValidStockQuotes[6].Close, ValidStockQuotes[6].Volume, },
        };

        public MatrixConvertTestData()
        {
            Add(new Tuple<List<StockQuote>, double[][]>(ValidStockQuotes, ExpectedResult));
        }
    }
}
