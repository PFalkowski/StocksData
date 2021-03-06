﻿using Stocks.Data.Common.Converters;
using Stocks.Data.UnitTests.Common.TestData;
using Xunit;

namespace Stocks.Data.UnitTests.Common
{
    public class RotateMatrixTest
    {

        [Theory]
        [ClassData(typeof(MatrixRotateTestData))]
        public void ConvertingWorks((double[][] input, double[][] expectedOutput) testFixture)
        {
            var tested = new RotateMatrix();
            var actual = tested.Convert(testFixture.input);
            Assert.Equal(testFixture.expectedOutput, actual);
        }
    }
}
