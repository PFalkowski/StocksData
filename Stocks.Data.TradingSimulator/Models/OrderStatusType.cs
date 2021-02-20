﻿namespace Stocks.Data.TradingSimulator.Models
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
