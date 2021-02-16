namespace Stocks.Data.Infrastructure
{
    public static class Constants
    {
        public const string StockQuoteName = "StockQuote";
        public const string CompanyName = "Company";
        public const string PreviousStockQuoteTickerName = "PreviousStockQuoteTicker";
        public const string PreviousStockQuoteDateName = "PreviousStockQuoteDate";

        public const string TickerName = "Ticker";
        public const string DateName = "Date";
        public const string OpenName = "Open";
        public const string HighName = "High";
        public const string LowName = "Low";
        public const string CloseName = "Close";
        public const string VolName = "Volume";

        public const string TickerNameRaw = "<TICKER>";
        public const string DateNameRaw = "<DTYYYYMMDD>";
        public const string OpenNameRaw = "<OPEN>";
        public const string HighNameRaw = "<HIGH>";
        public const string LowNameRaw = "<LOW>";
        public const string CloseNameRaw = "<CLOSE>";
        public const string VolNameRaw = "<VOL>";


        public const string TotalSharesEmittedName = "TotalSharesEmitted";
        public const string MarketCapName = "MarketCap";
        public const string BookValueName = "BookValue";
        public const string DividendYieldName = "DividendYield";
        public const string PriceToEarningsRatioName = "PriceToEarningsRatio";
        
        public const string BlacklistPatternString = @".*\d{3,}|WIG.*|RC.*|INTL.*|INTS.*|WIG.*|.*PP\d.*|.*BAHOLDING.*";
    }
}
