using System.Collections.Generic;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using NSubstitute;
using Stocks.Data.Model;
using Stocks.Data.Services;
using Stocks.Data.Services.Tier0;
using Xunit;

namespace Stocks.Data.UnitTests.Services
{
    public class StocksBulkDeserializerTest
    {
        [Fact]
        public void BulkDeserializer_WhenCreated_HasCorrectFields()
        {
            var deserializerMock = Substitute.For<IStocksDeserializer>();
            var tested = new StocksBulkDeserializer(deserializerMock);
            // Assert
            Assert.Equal(deserializerMock, tested.Deserializer);
        }

        [Fact]
        public void DeserializeParallel_GivenValidParameters_Deserializes()
        {
            const string ticer = "test123";
            var stubCompany = new Company { Ticker = ticer };
            var stubDict = new Dictionary<string, string>
            {
                {"test1", "test"},
                {"test2", "test"},
                {"test3", "test"},
                {"test4", "test"}
            };
            var deserializerMock = Substitute.For<IStocksDeserializer>();
            deserializerMock.Deserialize(Arg.Any<string>()).Returns(stubCompany);
            var tested = new StocksBulkDeserializer(deserializerMock);
            // Act
            var actual = tested.DeserializeParallel(stubDict);
            // Assert
            Assert.True(actual.TrueForAll(c => c.Ticker == ticer));
            Assert.Equal(stubDict.Count, actual.Count);
        }

        [Fact]
        public async Task DeserializeAsync_GivenValidParameters_Deserializes()
        {
            const string ticer = "test123";
            var stubCompany = new Company { Ticker = ticer };
            var stubDict = new Dictionary<string, string>
            {
                {"test1", "test"},
                {"test2", "test"},
                {"test3", "test"},
                {"test4", "test"}
            };
            var deserializerMock = Substitute.For<IStocksDeserializer>();
            deserializerMock.DeserializeAsync(Arg.Any<string>()).Returns(stubCompany);
            var tested = new StocksBulkDeserializer(deserializerMock);
            // Act
            var actual = await tested.DeserializeAsync(stubDict);
            // Assert
            Assert.True(actual.TrueForAll(c => c.Ticker == ticer));
            Assert.Equal(stubDict.Count, actual.Count);
        }

        [Fact]
        public void DeserializeGivenValidParametersDeserializes()
        {
            // Arrange
            var input = @"06MAGNA,20210219,2.4400,2.5800,2.3200,2.5200,81891
11BIT,20210219,565.0000,568.0000,550.0000,560.0000,5269
3RGAMES,20210219,1.0000,1.0200,1.0000,1.0200,11933";
            var output = new List<StockQuote>
            {
                new StockQuote{Ticker = "test1"},
                new StockQuote{Ticker = "test2"},
                new StockQuote{Ticker = "test3"},
            };
            var deserializerMock = Substitute.For<IStocksDeserializer>();
            deserializerMock.DeserializeQuotes(Arg.Any<string>()).Returns(output);
            var tested = new StocksBulkDeserializer(deserializerMock);

            // Act
            var actual = tested.Deserialize(input);

            // Assert
            Assert.True(actual.TrueForAll(c => c.Ticker.StartsWith("test")));
            Assert.Equal(3, actual.Count);
        }
    }
}
