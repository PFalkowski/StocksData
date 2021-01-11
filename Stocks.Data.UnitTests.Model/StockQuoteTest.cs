using Extensions.Standard;
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
            var tested = new StockQuote {Ticker = "IDK"};
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
            Assert.False(Equals(quote1, quote2));
            Assert.False(quote1.Equals(quote2));
            Assert.False(quote2.Equals(quote1));
        }
        [Theory]
        [ClassData(typeof(UnequalQuotePairsProvider))]
        public void GetHashCodeReturnsDifferentForUnequalStocks(StockQuote quote1, StockQuote quote2)
        {
            Assert.NotEqual(quote1.GetHashCode(), quote2.GetHashCode());
        }

        [Theory]
        [ClassData(typeof(ValidStockQuoteProvider))]
        public void PriceToBookValueRatio_Calculates_Correctly(StockQuote input)
        {
            Assert.Equal(1.13, input.PriceToBookValueRatio, 2);
        }
        [Theory]
        [ClassData(typeof(ValidStockQuoteProvider))]
        public void LastYearYield_Calculates_Correctly(StockQuote input)
        {
            Assert.True(input.LastYearYield.InClosedRange(107900000, 107990000));
        }
        [Theory]
        [ClassData(typeof(ValidStockQuoteProvider))]
        public void BookValuePerShare_Calculates_Correctly(StockQuote input)
        {
            Assert.Equal(103.18, input.BookValuePerShare, 1);
        }
        [Theory]
        [ClassData(typeof(ValidStockQuoteProvider))]
        public void EarningsPerShare_Calculates_Correctly(StockQuote input)
        {
            Assert.Equal(13.877, input.EarningsPerShare, 1);
        }
    }
}
