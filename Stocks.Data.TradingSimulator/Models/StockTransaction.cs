using System;
using System.Collections.Generic;
using System.Text;

namespace Stocks.Data.TradingSimulator.Models
{
    public class StockTransaction
    {
        public StockTransactionType TransactionType { get; set; }
        public double Price { get; set; }
        public double Volume { get; set; }
        public DateTime Date { get; set; }
    }
}
