using System;
using System.Collections.Generic;
using System.Text;

namespace Stocks.Data.TradingSimulator.Models
{
    public enum OrderStatusType
    {
        Unknown,
        Accepted,
        DeniedInsufficientFunds,
        DeniedOutOfRange,
        DeniedNoOpenPosition
    }
}
