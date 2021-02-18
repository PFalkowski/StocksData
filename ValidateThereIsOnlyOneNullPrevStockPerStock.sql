select count(Ticker), Ticker from StockQuote where PreviousStockQuoteDate is NULL or PreviousStockQuoteTicker is NULL
group by Ticker