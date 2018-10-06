using Microsoft.EntityFrameworkCore;

namespace Stocks.Data.Services
{
    public interface IDatabaseService
    {
        bool SetupDatabase(DbContextOptions context);
        bool DropDatabase(DbContextOptions context);
    }
}
