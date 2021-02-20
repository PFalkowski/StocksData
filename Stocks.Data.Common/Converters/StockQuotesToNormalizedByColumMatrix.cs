using System.Collections.Generic;
using StandardInterfaces;
using Stocks.Data.Model;

namespace Stocks.Data.Common.Converters
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
