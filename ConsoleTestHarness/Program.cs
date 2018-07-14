﻿using System;
using System.Collections.Generic;
using System.IO;
using Microsoft.EntityFrameworkCore;
using Stocks.Data;
using Stocks.Data.Ef;
using Stocks.Data.Infrastructure;
using Stocks.Data.Model;

namespace ConsoleApp1
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
            };
            return testStock;
        }

        private static DbContextOptions<DbContext> GetOptions()
        {
            string connectionStr = $"server=(localdb)\\MSSQLLocalDB;Initial Catalog={Path.GetFileNameWithoutExtension(Path.GetRandomFileName())};Integrated Security=True;";
            var options = new DbContextOptionsBuilder<DbContext>()
                .UseSqlServer(connectionStr)
                .Options;
            return options;
        }

        static void Main(string[] args)
        {
            var options = GetOptions();
            var input = GetStub();

            DbContext context = null;
            StockUnitOfWork tested = null;
            try
            {
                context = new StockContext(options);
                context.Database.EnsureCreated();
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
