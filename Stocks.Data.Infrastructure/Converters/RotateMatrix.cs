using StandardInterfaces;
using System.Collections.Generic;

namespace Stocks.Data.Infrastructure.Converters
{
    public class RotateMatrix : IConverter<double[][], double[][]>
    {
        public double[][] Convert(double[][] input)
        {
            int xLength = input.Length;
            if (xLength == 0)
            {
                return default(double[][]);
            }

            int yLength = input[0].Length;
            if (yLength == 0)
            {
                return default(double[][]);
            }

            double[][] result = new double[yLength][];

            for (int y = 0; y < yLength; ++y)
            {
                var temp = new List<double>(yLength);

                for (int x = 0; x < xLength; ++x)
                {
                    temp.Add(input[x][y]);
                }
                result[y] = temp.ToArray();
            }
            return result;
        }
    }
}
