﻿using Stocks.Data.Infrastructure.Converters;
using Stocks.Data.UnitTests.Infrastructure.Mocks;
using System;
using System.Collections.Generic;
using System.Text;
using Xunit;

namespace Stocks.Data.UnitTests.Infrastructure
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