using Stocks.Data.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Stocks.Data.Test.Mocks;
using Xunit;

namespace Stocks.Data.Test
{
    public class StockQuotesToMatrixConverterTest
    {
        [Theory]
        [ClassData(typeof(ValidQuotesWithExpected))]
        public void ConvertingWorks(Tuple<List<StockQuote>, double[][]> input)
        {
            var tested = new StockQuotesToMatrixConverter();
            var actual = tested.Convert(input.Item1).ToList();

            for (var i = 0; i < actual.Count; ++i)
            {
                Assert.Equal(input.Item2[i][0], actual[i][0]);
                Assert.Equal(input.Item2[i][1], actual[i][1]);
                Assert.Equal(input.Item2[i][2], actual[i][2]);
                Assert.Equal(input.Item2[i][3], actual[i][3]);
                Assert.Equal(input.Item2[i][4], actual[i][4]);
            }
        }
    }
}
