using LoggerLite;
using Stocks.Data.Api.Services;
using Stocks.Data.Model;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using SimpleInjector;
using Stocks.Data.Api.TestHarness.Startup;
using Stocks.Data.Common.Models;
using static ConsoleUserInteractionHelper.ConsoleHelper;

namespace Stocks.Data.Api.TestHarness
{
    class Program
    {
        private static readonly Container _container = new Container();
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

        static async Task Main(string[] args)
        {
            SimpleInjectorInitialize.Initialize(_container, null);

            var api = _container.GetInstance<IStockQuotesMigrationFromCsv>();
            var quotesDownloader = _container.GetInstance<IStockQuotesDownloadService>();
            var dbManagementSvc = _container.GetInstance<IDatabaseManagementService>();
            var logger = _container.GetInstance<ILogger>();
            var dbName = Path.GetFileNameWithoutExtension(Path.GetRandomFileName());
            var project = new ProjectSettings
            {
                ConnectionString =
                    $"server=(localdb)\\MSSQLLocalDB;Initial Catalog={dbName};Integrated Security=True;"
            };

            try
            {
                var input = GetStub();


                logger.LogInfo($"Hello in {project.Name}. This test will read a directory and load it into Dictionary of deserialized stock quotes.");
                logger.LogInfo($"Would you like to download latest stocks from {project.QuotesDownloadUrl} ? (y/n)");
                var response = GetBinaryDecisionFromUser();

                if (response)
                {
                    await quotesDownloader.Download(project);
                }

                logger.LogInfo($"Would you like to upload quotes to database? (y/n)");
                response = GetBinaryDecisionFromUser();

                if (response)
                {
                    await api.Migrate(project, TargetLocation.ZipArchive);
                }
            }
            finally
            {
                await dbManagementSvc.EnsureDbDoesNotExist(project);
            }
        }
    }
}
