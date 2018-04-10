using System;
using Stocks.Data.Model.Test.Mocks;
using Xunit;

namespace Stocks.Data.Model.Test
{
    public class StockQuoteTest
    {
        [Fact]
        public void CanCreateInstance()
        {
            var tested = new StockQuote();
            tested.Ticker = "IDK";
        }
        [Theory]
        [ClassData(typeof(ValidQuotesProvider))]
        public void IsValidIsTrueForValid(StockQuote inputQuote)
        {
            Assert.True(inputQuote.IsValid());
        }
        [Theory]
        [ClassData(typeof(InvalidQuotesProvider))]
        public void IsValidIsFalseForInvalid(StockQuote inputQuote)
        {
            Assert.False(inputQuote.IsValid());
        }
    }
}
