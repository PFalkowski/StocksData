using StandardInterfaces;
using Stocks.Data.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using OnTheFlyStats;
using Extensions.Standard;

namespace Stocks.Data.Infrastructure.Converters
{
    public class MatrixToNormalizedByColumnMatrix : IConverter<double[][], double[][]>
    {
        public double[][] Convert(double[][] matrix)
        {

            const double normalizedHigh = 1.0;
            const double normalizedLow = 0.0;

            var openNormalized = default(double[]);
            var highNormalized = default(double[]);
            var lowNormalized = default(double[]);
            var closeNormalized = default(double[]);
            var volNormalized = default(double[]);

            Parallel.Invoke(
                () => openNormalized = matrix[0].Scale(normalizedHigh, normalizedLow).ToArray(),
                () => highNormalized = matrix[1].Scale(normalizedHigh, normalizedLow).ToArray(),
                () => lowNormalized = matrix[2].Scale(normalizedHigh, normalizedLow).ToArray(),
                () => closeNormalized = matrix[3].Scale(normalizedHigh, normalizedLow).ToArray(),
                () => volNormalized = matrix[4].Scale(normalizedHigh, normalizedLow).ToArray());

            return new double[][] { openNormalized, highNormalized, lowNormalized, closeNormalized, volNormalized };
        }
    }
}
