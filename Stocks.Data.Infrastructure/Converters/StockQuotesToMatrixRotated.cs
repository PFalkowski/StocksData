using System.Collections.Generic;
using StandardInterfaces;
using Stocks.Data.Model;

namespace Stocks.Data.Infrastructure.Converters
{
    public class StockQuotesToMatrixRotated : IConverter<List<StockQuote>, double[][]>
    {
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

            return new double[][] { opens, highs, lows, closes, vols };
        }
    }
}
