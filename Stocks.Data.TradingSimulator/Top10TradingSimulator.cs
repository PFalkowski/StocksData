using LoggerLite;
using Stocks.Data.Common.Models;
using Stocks.Data.Ef;
using Stocks.Data.Model;
using Stocks.Data.TradingSimulator.Models;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Stocks.Data.TradingSimulator
{
    public class Top10TradingSimulator : TradingSimulatorBase
    {
        public Top10TradingSimulator(ILogger logger,
            IStockQuoteRepository stockQuoteRepository, 
            IProjectSettings projectSettings)
            : base(logger, stockQuoteRepository, projectSettings) { }

        protected override List<StockQuote> GetTopN(ITradingSimulationConfig tradingSimulationConfig, List<StockQuote> allQuotesPrefilterd, DateTime date)
        {
            var allQuotesBeforeTradeDay = allQuotesPrefilterd.Where(x => x.DateParsed.Date < date.Date).ToList();
            var nMinusOneDay = allQuotesBeforeTradeDay.Select(x => x.DateParsed).Max();
            var allQuotesFromMinusOneDay = allQuotesPrefilterd.Where(x => x.DateParsed.Date.Equals(nMinusOneDay.Date)).ToList();

            var topN = allQuotesFromMinusOneDay
                .OrderByDescending(x => x.AveragePriceChange)
                .Take(tradingSimulationConfig.TopN)
                .ToList();

            return topN;
        }
    }
}
