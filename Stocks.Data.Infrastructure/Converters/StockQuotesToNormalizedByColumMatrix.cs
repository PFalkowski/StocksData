using StandardInterfaces;
using Stocks.Data.Model;
using System.Collections.Generic;

namespace Stocks.Data.Infrastructure.Converters
{
    public class StockQuotesToNormalizedByColumMatrix : IConverter<List<StockQuote>, double[][]>
    {
        public double[][] Convert(List<StockQuote> input)
        {
            var matrixConverterRotator = new StockQuotesToMatrixRotated();
            var matrixNormalizer = new NormalizeByColumn();

            var matrixRotated = matrixConverterRotator.Convert(input);
            var normalized = matrixNormalizer.Convert(matrixRotated);
            return normalized;
        }
    }
}
