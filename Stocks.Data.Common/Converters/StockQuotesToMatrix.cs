using System.Collections.Generic;
using StandardInterfaces;
using Stocks.Data.Model;

namespace Stocks.Data.Common.Converters
{
    public class StockQuotesToMatrix : IConverter<List<StockQuote>, double[][]>
    {
        /// <summary>
        /// <para>[0][0] = Open1,  [0][1] = High1,  [0][2] = Low1,  [0][3] = Close1,  [0][4] = Vol1</para>
        /// <para>[1][0] = Open2,  [1][1] = High2,  [1][2] = Low2,  [1][3] = Close2,  [1][4] = Vol2</para>
        /// <para>[2][0] = Open3,  [2][1] = High3,  [2][2] = Low3,  [2][3] = Close3,  [2][4] = Vol3</para>
        /// </summary>
        /// <param name="input"></param>
        /// <returns>new double[][] { sample1, sample2, sample3... }</returns>
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
