﻿using Microsoft.EntityFrameworkCore;
using Stocks.Data.Model;

namespace Stocks.Data.Ef.Repositories
{
    public class TradingSimulationResultRepository :  Repository<TradingSimulationResult>, ITradingSimulationResultRepository
    {
        public TradingSimulationResultRepository(DbContext context) : base(context)
        {
        }
    }
}
