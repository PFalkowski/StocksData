using System;
using System.Threading.Tasks;
using LoggerLite;
using Microsoft.EntityFrameworkCore;
using Stocks.Data.Common.Models;
using Stocks.Data.Ef;

namespace Stocks.Data.Services.Tier1
{
    public class MsSqlDatabaseManagementService : IDatabaseManagementService
    {
        private readonly ILogger _logger;

        public MsSqlDatabaseManagementService(ILogger logger)
        {
            _logger = logger;
        }

        public async Task<bool> EnsureDbExists(IProjectSettings project, int retries = 3)
        {
            DbContext context = null;
            var result = false;
            try
            {
                context = new StockContext(project);
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
        
        public async Task<bool> EnsureDbDoesNotExist(IProjectSettings project, int retries = 3)
        {
            DbContext context = null;
            var result = false;
            try
            {
                context = new StockContext(project);
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
