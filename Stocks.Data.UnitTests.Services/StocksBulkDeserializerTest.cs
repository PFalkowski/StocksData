using System.Collections.Generic;
using System.Threading.Tasks;
using NSubstitute;
using Stocks.Data.Model;
using Stocks.Data.Services;
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
        public void Deserialize_GivenValidParameters_Deserializes()
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
            var actual = tested.Deserialize(stubDict);
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
    }
}
