using System;
using System.Globalization;
using System.Linq;
using System.Text;
using Extensions.Serialization;
using Stocks.Data.Infrastructure;
using Stocks.Data.Model;
using Xunit;

namespace Stocks.Data.Ef.Test.Mocks
{
    public  class _11BitMock : TheoryData<Company>
    {
        private static Lazy<Company> Mock => new Lazy<Company>(() => new Company
        {
            Ticker = nameof(Properties.Resources._11BIT),
            Quotes = Encoding.UTF8.GetString(Properties.Resources._11BIT).DeserializeFromCsv(new StockQuoteCsvClassMap(), CultureInfo.InvariantCulture).ToList()
        });

        public _11BitMock()
        {
            Add(Mock.Value);
        }
    }
}
