using System;
using System.Collections.Generic;
using System.Linq;
using Stocks.Data.Infrastructure.Converters;
using Stocks.Data.UnitTests.Infrastructure.Mocks;
using Stocks.Data.Model;
using Xunit;

namespace Stocks.Data.UnitTests.Infrastructure
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
