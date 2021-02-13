using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using LoggerLite;
using Microsoft.EntityFrameworkCore;
using Stocks.Data.Api.Services;
using Stocks.Data.Common.Models;
using Stocks.Data.Ef;
using Stocks.Data.Model;

namespace Stocks.Data.ConsoleTestHarness
{
    class Program
    {
        private static ILogger _logger = new ConsoleLogger();
        private static readonly IDatabaseManagementService DbManagementService = new MsSqlDatabaseManagementService(_logger);
        private static Company GetStub()
        {
            const string ticker = "test";
            var testQuote = new StockQuote
            {
                Close = 10,
                Date = 20001010,
                Ticker = ticker,
                High = 10,
                Low = 10,
                Volume = 1000,
                Open = 10,
                TotalSharesEmitted = 100000,
                MarketCap = 200000,
                BookValue = 1000,
                DividendYield = 1.0,
                PriceToEarningsRatio = 2
            };
            var testStock = new Company
            {
                Quotes = new List<StockQuote> { testQuote },
                Ticker = ticker
            };
            return testStock;
        }

        static async Task Main()
        {
            var dbName = Path.GetFileNameWithoutExtension(Path.GetRandomFileName());
            var proj = new ProjectSettings
            {
                ConnectionString =
                    $"server=(localdb)\\MSSQLLocalDB;Initial Catalog={dbName};Integrated Security=True;"
            };
            var input = GetStub();

            DbContext context = null;
            StockUnitOfWork tested = null;
            try
            {
                var res = await DbManagementService.EnsureDbExists(proj);
                context = new StockContext(proj);
                tested = new StockUnitOfWork(context);

                tested.StockRepository.Add(input);
                tested.Complete();

                var actual = tested.StockRepository.Count();

                Console.WriteLine($"Added {actual} record(s) to db");
                Console.ReadKey();
                await DbManagementService.EnsureDbDoesNotExist(proj);
            }
            finally
            {
                tested?.Dispose();
                if (context != null)
                    await context.DisposeAsync();
            }
        }
    }
}
