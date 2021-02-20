using System;
using System.Collections.Generic;
using System.Linq;
using Stocks.Data.Common.Converters;
using Stocks.Data.Model;
using Stocks.Data.UnitTests.Common.TestData;
using Xunit;

namespace Stocks.Data.UnitTests.Common
{
    public class StockQuotesToMatrixTest
    {
        [Theory]
        [ClassData(typeof(MatrixConvertTestData))]
        public void ConvertingWorks(Tuple<List<StockQuote>, double[][]> input)
        {
            var tested = new StockQuotesToMatrix();
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
