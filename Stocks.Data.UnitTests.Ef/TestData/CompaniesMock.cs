using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using Extensions.Serialization.Csv;
using Stocks.Data.Infrastructure;
using Stocks.Data.Model;
using Xunit;

namespace Stocks.Data.UnitTests.Ef.Test.TestData
{
    public class CompaniesMock : TheoryData<List<Company>>
    {
        public CompaniesMock()
        {
            var resultList = new List<Company>();
            var resourceSet =
                Properties.Resources.ResourceManager.GetResourceSet(CultureInfo.CurrentUICulture, true, true);
            foreach (DictionaryEntry entry in resourceSet)
            {
                var resourceKey = entry.Key.ToString();
                var resource = entry.Value;

                var quotes = Encoding.UTF8.GetString((byte[])resource)
                    .DeserializeFromCsv(new StockQuoteCsvClassMap(), CultureInfo.InvariantCulture).ToList();
                for (var i = 1; i < quotes.Count; i++)
                {
                    quotes[i].PreviousStockQuote = quotes[i - 1];
                    quotes[i].DateParsed = DateTime.ParseExact(quotes[i].Date.ToString(), "yyyyMMdd", CultureInfo.InvariantCulture);
                    quotes[i].AveragePriceChange = (quotes[i].AveragePrice - quotes[i - 1].AveragePrice) /
                                                   quotes[i - 1].AveragePrice;
                }
                resultList.Add(
                new Company
                {
                    Ticker = resourceKey,
                    Quotes = quotes
                });
            }
            Add(resultList);
        }
    }
}
