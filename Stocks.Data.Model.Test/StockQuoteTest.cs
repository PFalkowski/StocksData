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
        //[Theory]
        //[TheoryData(typeof(ValidQuotesProvider))]
        //public void IsValidIsFalseForInvalid(StockQuote inputQuote)
        //{

        //}
        [Fact]
        public void IsValidIsTrueForValid()
        {

        }
    }
}
