select count(Ticker), Ticker from StockQuote where AveragePriceChange is NULL
group by Ticker