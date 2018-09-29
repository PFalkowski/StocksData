using Microsoft.EntityFrameworkCore;
using System;

namespace Stocks.Data.Service
{
    public interface IDatabaseService
    {
        bool SetupDatabase(DbContextOptions context);
        bool DropDatabase(DbContextOptions context);
    }
}
