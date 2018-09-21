using StandardInterfaces;
using Stocks.Data.Model;
using System.Collections.Generic;

namespace Stocks.Data.Infrastructure.Converters
{
    public class StockQuotesToMatrix : IConverter<List<StockQuote>, double[][]>
    {
        public double[][] Convert(List<StockQuote> input)
        {
            double[][] result = new double[input.Count][];
            for (int i = 0; i < input.Count; ++i)
            {
                result[i] = new[]
                {
                    input[i].Open,
                    input[i].High,
                    input[i].Low,
                    input[i].Close,
                    input[i].Volume
                };
            }
            return result;
        }
    }
}
