using CsvHelper.Configuration;
using Stocks.Data.Infrastructure;
using Stocks.Data.Model;

namespace Stocks.Data.Csv
{
    public sealed class StockQuoteCsvClassMap : ClassMap<StockQuote>
    {
        public StockQuoteCsvClassMap()
        {
            Map(m => m.Ticker).Name(Constants.TickerNameRaw);
            Map(m => m.Date).Name(Constants.DateNameRaw);
            Map(m => m.Open).Name(Constants.OpenNameRaw);
            Map(m => m.High).Name(Constants.HighNameRaw);
            Map(m => m.Low).Name(Constants.LowNameRaw);
            Map(m => m.Close).Name(Constants.CloseNameRaw);
            Map(m => m.Volume).Name(Constants.VolNameRaw);
            Map(m => m.DateParsed).Ignore();
        }
    }
}
