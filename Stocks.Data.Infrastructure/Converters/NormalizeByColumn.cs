using Extensions.Standard;
using StandardInterfaces;
using System.Linq;

namespace Stocks.Data.Infrastructure.Converters
{
    public class NormalizeByColumn : IConverter<double[][], double[][]>
    {
        public double NormalizedMax { get; set; } = 1.0;
        public double NormalizedMin { get; set; } = 0.0;

        public double[][] Convert(double[][] matrix)
        {
            int xLength = matrix.Length;
            if (xLength == 0)
            {
                return default(double[][]);
            }

            int yLength = matrix[0].Length;
            if (yLength == 0)
            {
                return default(double[][]);
            }

            double[][] result = new double[xLength][];

            for (int i = 0; i < xLength; ++i)
            {
                result[i] = matrix[i].Scale((Min: NormalizedMin, Max: NormalizedMax)).ToArray();
            }

            return result;
        }
    }
}
