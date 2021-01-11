using LoggerLite;
using Microsoft.EntityFrameworkCore;
using Stocks.Data.Ef;
using System;
using System.Threading.Tasks;
using Stocks.Data.Api.Models;

namespace Stocks.Data.Api.Services
{
    public class MsSqlDatabaseManagementService : IDatabaseManagementService
    {
        private readonly ILogger _logger;

        public MsSqlDatabaseManagementService(ILogger logger)
        {
            _logger = logger;
        }

        public async Task<bool> EnsureDbExists(Project project, int retries = 3)
        {
            var options = GetOptions(project.ConnectionString);

            DbContext context = null;
            var result = false;
            try
            {
                context = new StockContext(options);
                var i = 0;
                while (!result && i < retries)
                {
                    ++i;
                    result = await context.Database.EnsureCreatedAsync();
                }

                if (result)
                {
                    _logger?.LogInfo("Created database.");
                }
                else
                {
                    _logger?.LogWarning($"Could not create database after {i} retires.");
                }
            }
            catch (Exception e)
            {
                _logger?.LogError(e);
            }
            finally
            {
                if (context != null)
                {
                    await context.DisposeAsync();
                }
            }

            return result;
        }

        private static DbContextOptions<DbContext> GetOptions(string connectionStr)
        {
            var options = new DbContextOptionsBuilder<DbContext>()
                .UseSqlServer(connectionStr)
                .Options;
            return options;
        }

        public async Task<bool> DropLocalDbAsync(Project project, int retries = 3)
        {
            var options = GetOptions(project.ConnectionString);

            DbContext context = null;
            var result = false;
            try
            {
                context = new StockContext(options);
                var i = 0;
                while (!result && i < retries)
                {
                    ++i;
                    result = await context.Database.EnsureDeletedAsync();
                }

                if (result)
                {
                    _logger?.LogInfo("Dropped database.");
                }
                else
                {
                    _logger?.LogWarning($"Could not drop database after {i} retires.");
                }
                return result;
            }
            catch (Exception e)
            {
                _logger?.LogError(e);
            }
            finally
            {
                if (context != null)
                {
                    await context.DisposeAsync();
                }
            }

            return result;
        }
    }
}
