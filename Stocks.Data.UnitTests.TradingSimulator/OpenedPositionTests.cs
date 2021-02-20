using System;
using NUnit.Framework;
using Stocks.Data.TradingSimulator.Models;

namespace Stocks.Data.UnitTests.TradingSimulator
{
    [TestFixture]
    public class OpenedPositionTests
    {
        [Test]
        public void IncreaseCorrectlyIncreasesVolAndPrice()
        {
            // Arrange
            var price1 = 10;
            var price2 = 20;

            var volume1 = 10;
            var volume2 = 10;

            var tested = new OpenedPosition { Price = price1, Volume = volume1 };

            // Act

            tested.Increase(volume2, price2);

            // Assert

            Assert.AreEqual(15, tested.Price);
            Assert.AreEqual(20, tested.Volume);
        }

        [Test]
        public void PlusOperatorCorrectlyIncreasesVolAndPrice()
        {
            // Arrange
            var price1 = 10;
            var price2 = 20;

            var volume1 = 10;
            var volume2 = 10;
            
            var tested = new OpenedPosition { Price = price1, Volume = volume1 };
            var tested2 = new OpenedPosition { Price = price2, Volume = volume2 };

            // Act

            tested += tested2;

            // Assert

            Assert.AreEqual(15, tested.Price);
            Assert.AreEqual(20, tested.Volume);
        }
        
        [Test]
        public void DecreaseCorrectlyDecreasesVol()
        {
            // Arrange
            var price1 = 10;

            var volume1 = 10;
            var volume2 = 10;

            var tested = new OpenedPosition { Price = price1, Volume = volume1 };

            // Act

            tested.Decrease(volume2);

            // Assert

            Assert.AreEqual(price1, tested.Price);
            Assert.AreEqual(0, tested.Volume);
        }
        
        [Test]
        public void DecreaseCorrectlyDecreasesVolWhenNotAllSold()
        {
            // Arrange
            var price1 = 10;

            var volume1 = 20;
            var volume2 = 10;

            var tested = new OpenedPosition { Price = price1, Volume = volume1 };

            // Act

            tested.Decrease(volume2);

            // Assert

            Assert.AreEqual(price1, tested.Price);
            Assert.AreEqual(10, tested.Volume);
        }

        [Test]
        public void DecreaseThrowsWhenVolumeLessThenOvned()
        {
            // Arrange
            var price1 = 10;

            var volume1 = 10;
            var volume2 = 20;

            var tested = new OpenedPosition { Price = price1, Volume = volume1 };

            // Act & Assert

            Assert.Throws<ArgumentOutOfRangeException>(() => tested.Decrease(volume2));
        }
    }
}
