using System;
using System.Globalization;
using System.Linq;
using System.Text;
using Extensions.Serialization;
using Stocks.Data.Infrastructure;
using Stocks.Data.Model;
using Xunit;

namespace Stocks.Data.UnitTests.Ef.Test.Mocks
{
    public class CdProjectMock : TheoryData<Company>
    {
        private static Lazy<Company> Mock => new Lazy<Company>(() => new Company
        {
            Ticker = nameof(Properties.Resources.CDPROJEKT),
            Quotes = Encoding.UTF8.GetString(Properties.Resources.CDPROJEKT).DeserializeFromCsv(new StockQuoteCsvClassMap(), CultureInfo.InvariantCulture).ToList()
        });

        public CdProjectMock()
        {
            Add(Mock.Value);
        }
    }
}
