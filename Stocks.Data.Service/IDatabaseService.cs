using System;

namespace Stocks.Data.Service
{
    public interface IDatabaseService
    {
        bool CreateEmptyDatabase(string connectionString);
        int RunMigration();
        bool DropDatabase();
    }
}
