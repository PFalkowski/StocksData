using NSubstitute;
using Services.IO;
using Stocks.Data.Model;
using System;
using System.Collections.Generic;
using Stocks.Data.Services;
using Xunit;

namespace Stocks.Data.UnitTests.Services
{
    public class StocksFileProviderTest
    {
        [Fact]
        public void ReadStocksFromReads()
        {
            // Arrange
            const string directory = "testDirectory";
            const string pattern = "testPattern";

            var stocksDict = new Dictionary<string, string> { { "testKey1", "testValue2" } };
            var stocksDirectoryProviderMock = Substitute.For<IDirectoryService>();
            stocksDirectoryProviderMock.ReadTopDirectory(Arg.Is(directory), Arg.Is(pattern)).Returns(stocksDict);

            var expectedCompanyList = new List<Company>();
            var stocksDeserializationMock = Substitute.For<IStocksBulkDeserializer>();
            stocksDeserializationMock.Deserialize(Arg.Is(stocksDict)).Returns(expectedCompanyList);

            var tested = new StocksFileProvider(stocksDirectoryProviderMock, stocksDeserializationMock);

            // Act
            var received = tested.ReadStocksFrom(directory, pattern);

            // Assert
            Assert.Equal(expectedCompanyList, received);
        }
        [Fact]
        public void StocksFileProviderValidatesConstruction()
        {
            var stocksDeserializationMock = Substitute.For<IStocksBulkDeserializer>();
            Assert.Throws<InvalidOperationException>(() => new StocksFileProvider(null, stocksDeserializationMock));
        }
        [Fact]
        public void StocksFileProviderValidatesConstruction2()
        {
            var stocksDirectoryProviderMock = Substitute.For<IDirectoryService>();
            Assert.Throws<InvalidOperationException>(() => new StocksFileProvider(stocksDirectoryProviderMock, null));
        }
    }
}
