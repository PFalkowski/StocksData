using System;
using System.Collections.Generic;
using System.IO;
using Microsoft.EntityFrameworkCore;
using Stocks.Data.Ef;
using Stocks.Data.Model;

namespace Stocks.Data.ConsoleTestHarness
{
    class Program
    {
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
                Open = 10
            };
            var testStock = new Company
            {
                Quotes = new List<StockQuote> { testQuote },
                Ticker = ticker
            };
            return testStock;
        }

        private static DbContextOptions<DbContext> GetOptions(string connectionStr)
        {
            var options = new DbContextOptionsBuilder<DbContext>()
                .UseSqlServer(connectionStr)
                .Options;
            return options;
        }

        static void Main()
        {
            var dbName = Path.GetFileNameWithoutExtension(Path.GetRandomFileName());
            string cs = $"server=(localdb)\\MSSQLLocalDB;Initial Catalog={dbName};Integrated Security=True;";

            var options = GetOptions(cs);
            var input = GetStub();

            DbContext context = null;
            StockUnitOfWork tested = null;
            try
            {
                context = new StockContext(options);
                context.Database.EnsureCreated();
                Console.WriteLine($"Created database {dbName}");
                tested = new StockUnitOfWork(context);

                tested.StockRepository.Add(input);
                tested.Complete();

                var actual = tested.StockRepository.Count();

                Console.WriteLine($"Added {actual} record(s) to db");
                Console.ReadKey();
            }
            finally
            {
                context?.Database.EnsureDeleted();
                tested?.Dispose();
                context?.Dispose();
            }
        }
    }
}
