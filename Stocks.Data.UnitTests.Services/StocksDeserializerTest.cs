using System;
using System.Text;
using CsvHelper.Configuration;
using NSubstitute;
using Stocks.Data.Model;
using Stocks.Data.Services;
using Stocks.Data.UnitTests.Services.Mocks;
using Xunit;

namespace Stocks.Data.UnitTests.Services
{
    public class StocksDeserializerTest
    {
        [Fact]
        public void Deserializer_WhenCreated_HasDefaultValues()
        {
            var mapMock = new MockStockClassMap();
            var tested = new StocksDeserializer(mapMock);
            Assert.Equal(System.Globalization.CultureInfo.InvariantCulture, tested.Culture);
        }
        [Fact]
        public void Deserializer_WithValidParameters_Deserializes()
        {
            var mapMock = new MockStockClassMap();
            var tested = new StocksDeserializer(mapMock);
            var cdProject = Encoding.UTF8.GetString( Properties.Resources.CDPROJEKT);

            var result = tested.Deserialize(cdProject);
            Assert.True(result.Quotes.TrueForAll(q => q.Ticker.Equals("CDPROJEKT", StringComparison.InvariantCultureIgnoreCase)));
        }
    }
}
