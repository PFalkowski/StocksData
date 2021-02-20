using System;
using System.Collections.Generic;
using System.Linq;
using Stocks.Data.Common.Converters;
using Stocks.Data.Model;
using Stocks.Data.UnitTests.Common.TestData;
using Xunit;

namespace Stocks.Data.UnitTests.Common
{
    public class StockQuotesToMatrixRotatedTest
    {
        [Theory]
        [ClassData(typeof(RotatedTestData))]
        public void ConvertingWorks(Tuple<List<StockQuote>, double[][]> input)
        {
            var tested = new StockQuotesToMatrixRotated();
            var actual = tested.Convert(input.Item1).ToList();

            for (var i = 0; i < actual.Count; ++i)
            {
                for (int j = 0; j < 4; ++j)
                {
                    Assert.Equal(input.Item2[i][j], actual[i][j]);
                }
            }
        }
    }
}
