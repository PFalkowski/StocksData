using System;
using Microsoft.EntityFrameworkCore;

namespace Stocks.Data.Services
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
