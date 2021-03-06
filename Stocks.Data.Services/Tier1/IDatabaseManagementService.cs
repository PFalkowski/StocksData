﻿using System.Threading.Tasks;
using Stocks.Data.Common.Models;

namespace Stocks.Data.Services.Tier1
{
    public interface IDatabaseManagementService
    {
        Task<bool> EnsureDbExists(IProjectSettings project, int retries = 3);
        Task<bool> EnsureDbDoesNotExist(IProjectSettings project, int retries = 3);
    }
}