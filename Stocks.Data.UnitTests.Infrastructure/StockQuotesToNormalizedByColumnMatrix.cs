using System.Collections.Generic;
using Stocks.Data.Common.Converters;
using Stocks.Data.Model;
using Stocks.Data.UnitTests.Common.TestData;
using Xunit;

namespace Stocks.Data.UnitTests.Common
{
    public class StockQuotesToNormalizedByColumnMatrixTest
    {
        [Theory]
        [ClassData(typeof(NormalizedRotatedTestData))]
        public void ConvertingWorks((List<StockQuote> input, double[][] expectedOutput) testFixture)
        {
            var tested = new StockQuotesToNormalizedByColumMatrix();
            var actual = tested.Convert(testFixture.input);

            Assert.Equal(testFixture.expectedOutput.Length, actual.Length);

            for (var i = 0; i < actual.Length; ++i)
            {
                for (int j = 0; j < testFixture.expectedOutput[i].Length; ++j)
                {
                    Assert.Equal(testFixture.expectedOutput[i][j], actual[i][j], 5);
                }
            }

        }
    }
}
