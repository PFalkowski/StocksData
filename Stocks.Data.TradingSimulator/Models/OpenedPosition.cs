using System;

namespace Stocks.Data.TradingSimulator.Models
{
    public class OpenedPosition
    {
        public double Price { get; set; }
        public double Volume { get; set; }

        public static OpenedPosition operator +(OpenedPosition a, OpenedPosition b)
        {
            a.Increase(b.Volume, b.Price);

            return a;
        }

        public void Increase(double volume, double price)
        {
            Price = (Price * Volume + price * volume)
                    / (Volume + volume);
            Volume += volume;
        }

        public void Decrease(double volume)
        {
            if (volume > Volume)
            {
                throw new ArgumentOutOfRangeException(nameof(volume));
            }

            Volume -= volume;
        }
    }
}



