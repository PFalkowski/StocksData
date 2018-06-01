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
    class MbankMock : TheoryData<Company>
    {
        private static Lazy<Company> Mock => new Lazy<Company>(() => new Company
        {
            Ticker = nameof(Properties.Resources.MBANK),
            Quotes = Encoding.UTF8.GetString(Properties.Resources.MBANK).DeserializeFromCsv(new StockQuoteCsvClassMap(), CultureInfo.InvariantCulture).ToList()
        });

        public MbankMock()
        {
            Add(Mock.Value);
        }
    }
}
