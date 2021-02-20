using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using CsvHelper.Configuration;
using Extensions.Serialization.Csv;
using Stocks.Data.Model;

namespace Stocks.Data.Services.Tier0
{
    public class StocksDeserializer : IStocksDeserializer
    {
        public CultureInfo Culture { get; }
        public ClassMap<StockQuote> Map { get; }

        public StocksDeserializer(ClassMap<StockQuote> map, CultureInfo culture = null)
        {
            Culture = culture ?? CultureInfo.InvariantCulture;
            Map = map;
        }

        public Company Deserialize(string fileContents)
        {
            var deserializedQuotes = fileContents.DeserializeFromCsv(Map, Culture).ToList();
            var companyName = deserializedQuotes.First().Ticker;

            return new Company
            {
                Ticker = companyName,
                Quotes = deserializedQuotes
            };
        }
        public async Task<Company> DeserializeAsync(string fileContents)
        {
            var deserializedQuotes = await Task.Run(() => fileContents.DeserializeFromCsv(Map, Culture).ToList());
            var companyName = deserializedQuotes.First().Ticker;

            return new Company
            {
                Ticker = companyName,
                Quotes = deserializedQuotes
            };
        }
    }
}