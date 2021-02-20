using System.Collections.Generic;
using Stocks.Data.Model;
using Xunit;

namespace Stocks.Data.UnitTests.Common.TestData
{
    public class NormalizedRotatedTestData : TheoryData<(List<StockQuote> input, double[][] expectedOutput)>
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
            new[]{0.1011378003,0.3122629583,0,0.2490518331,0.3881163085,0.4943109987,1},
            new[]{0.2747791953,0.3631010795,0,0.3719332679,0.4700686948,0.5093228656,1},
            new[]{0,0.2898550725,0.02898550725,0,0.4347826087,0.5652173913,1},
            new[]{0.1179941003,0.2949852507,0,0.4424778761,0.4424778761,0.5575221239,1},
            new[]{0.004522613065,0.004522613065,0,0.09949748744,1,0.1457286432,0.4935678392}
        };

        public NormalizedRotatedTestData()
        {
            Add((input: ValidStockQuotes, expectedOutput: ExpectedResult));
        }
    }
}
