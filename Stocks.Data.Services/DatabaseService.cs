using Microsoft.EntityFrameworkCore;
using System;

namespace Stocks.Data.Service
{
    public class DatabaseService : IDatabaseService
    {
        public bool SetupDatabase(DbContextOptions context)
        {
            throw new NotImplementedException();
        }
        public bool DropDatabase(DbContextOptions context)
        {
            throw new NotImplementedException();
        }
    }
}
