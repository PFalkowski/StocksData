using System;
using System.Collections.Generic;
using System.Text;
using Extensions.Standard;
using Stocks.Data.Model;
using Stocks.Data.UnitTests.Model.TestData;
using Xunit;

namespace Stocks.Data.UnitTests.Model
{
    // https://dariuszgrupa.pl/jak-policzyc-wskazniki-dla-akcji-na-gieldzie/
    public class StockQuoteWithFundamentalsTest
    {

        [Theory]
        [ClassData(typeof(ValidStockQuoteWithFundamentalsProvider))]
        public void PriceToBookValueRatio_Calculates_Correctly(StockQuoteWithFundamentals input)
        {
            Assert.Equal(1.13, input.PriceToEarningsRatio);
        }
        [Theory]
        [ClassData(typeof(ValidStockQuoteWithFundamentalsProvider))]
        public void LastYearYield_Calculates_Correctly(StockQuoteWithFundamentals input)
        {
            Assert.True(input.LastYearYield.InClosedRange(107900000, 107990000));
        }
        [Theory]
        [ClassData(typeof(ValidStockQuoteWithFundamentalsProvider))]
        public void BookValuePerShare_Calculates_Correctly(StockQuoteWithFundamentals input)
        {
            Assert.Equal(103.18, input.BookValuePerShare, 1);
        }
        [Theory]
        [ClassData(typeof(ValidStockQuoteWithFundamentalsProvider))]
        public void EarningsPerShare_Calculates_Correctly(StockQuoteWithFundamentals input)
        {
            Assert.Equal(13.877, input.EarningsPerShare, 1);
        }
    }
}
