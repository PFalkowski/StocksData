using Xunit;

namespace Stocks.Data.UnitTests.Infrastructure.TestData
{
    public class MatrixRotateTestData : TheoryData<(double[][] input, double[][] expectedOutput)>
    {
        private readonly double[][] _input = new[]
        {
                new[] { 1.0, 2, 3 },
                new[] { 4.0, 5, 6 }
            };
        private readonly double[][] _expectedOutput = new[]
        {
                new[] { 1.0, 4 },
                new[] { 2.0, 5 },
                new[] { 3.0, 6 },
            };

        public MatrixRotateTestData()
        {
            Add((input: _input, expectedOutput: _expectedOutput));
        }
    }
}
