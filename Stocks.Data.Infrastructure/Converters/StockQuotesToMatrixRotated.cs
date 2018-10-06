using System.Collections.Generic;
using StandardInterfaces;
using Stocks.Data.Model;

namespace Stocks.Data.Infrastructure.Converters
{
    public class StockQuotesToMatrixRotated : IConverter<List<StockQuote>, double[][]>
    {

        /// <summary>
        /// <para>[0][0] = Open1,  [0][1] = Open2,  [0][2] = Open3...</para>
        /// <para>[1][0] = High1,  [1][1] = High2,  [1][2] = High3...</para>
        /// <para>[2][0] = Low1,   [2][1] = Low2,   [2][2] = Low3...</para>
        /// <para>[3][0] = Close1, [3][1] = Close2, [3][2] = Close3...</para>
        /// <para>[4][0] = Vol1,   [4][1] = Vol2,   [4][2] = Vol3...</para>
        /// </summary>
        /// <param name="input"></param>
        /// <returns>new double[][] { opens, highs, lows, closes, vols }</returns>
        public double[][] Convert(List<StockQuote> input)
        {
            var opens = new double[input.Count];
            var highs = new double[input.Count];
            var lows = new double[input.Count];
            var closes = new double[input.Count];
            var vols = new double[input.Count];

            for (var i = 0; i < input.Count; ++i)
            {
                opens[i] = input[i].Open;
                highs[i] = input[i].High;
                lows[i] = input[i].Low;
                closes[i] = input[i].Close;
                vols[i] = input[i].Volume;
            }

            return new[] { opens, highs, lows, closes, vols };
        }
    }
}
