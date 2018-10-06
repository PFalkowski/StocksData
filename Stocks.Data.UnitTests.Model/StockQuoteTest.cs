using Stocks.Data.Model;
using Stocks.Data.UnitTests.Model.TestData;
using Xunit;

namespace Stocks.Data.UnitTests.Model
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
        [Fact]
        public void ToStringReturnsTickerAndDate()
        {
            const string ticker = "IDK";
            const int date = 19900101;
            var tested = new StockQuote { Ticker = ticker, Date = date };
            Assert.Equal($"{ticker} {date}", tested.ToString());
        }
        [Theory]
        [ClassData(typeof(UnequalQuotePairsProvider))]
        public void EqualsReturnsFalseForUnequalStocks(StockQuote quote1, StockQuote quote2)
        {
            Assert.False(quote1 == quote2);
            Assert.False(object.Equals(quote1, quote2));
            Assert.False(quote1.Equals(quote2));
            Assert.False(quote2.Equals(quote1));
        }
        [Theory]
        [ClassData(typeof(UnequalQuotePairsProvider))]
        public void GetHashCodeReturnsDifferentForUnequalStocks(StockQuote quote1, StockQuote quote2)
        {
            Assert.NotEqual(quote1.GetHashCode(), quote2.GetHashCode());
        }
    }
}
