using System;
using System.Globalization;
using System.Linq;
using System.Text;
using Extensions.Serialization.Csv;
using Stocks.Data.Common;
using Stocks.Data.Model;
using Xunit;

namespace Stocks.Data.UnitTests.Ef.Test.TestData
{
    class MbankData : TheoryData<Company>
    {
        private static Lazy<Company> Mock => new Lazy<Company>(() => new Company
        {
            Ticker = nameof(Properties.Resources.MBANK),
            Quotes = Encoding.UTF8.GetString(Properties.Resources.MBANK).DeserializeFromCsv(new StockQuoteCsvClassMap(), CultureInfo.InvariantCulture).ToList()
        });

        public MbankData()
        {
            Add(Mock.Value);
        }
    }
}
