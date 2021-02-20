using System.Globalization;
using CsvHelper.Configuration;
using LoggerLite;
using Microsoft.Extensions.Configuration;
using Services.IO;
using SimpleInjector;
using Stocks.Data.Ado;
using Stocks.Data.Common;
using Stocks.Data.Model;
using Stocks.Data.Services;
using Stocks.Data.Services.Tier0;
using Stocks.Data.Services.Tier1;

namespace Stocks.Data.Api.TestHarness.Startup
{
    public static class SimpleInjectorInitialize
    {
        public static void Initialize(Container container, IConfiguration configuration)
        {
            container.Register<ILogger, ConsoleLogger>();
            container.Register<IDirectoryService, DirectoryService>();
            container.Register<IFileService, FileService>();
            container.Register<IUnzipper, Unzipper>();
            container.Register<IStocksDeserializer, StocksDeserializer>();
            container.Register<IStocksBulkDeserializer, StocksBulkDeserializer>();
            container.Register<BulkInserter<Company>, CompanyBulkInserter>();
            container.Register<BulkInserter<StockQuote>, StockQuotesBulkInserter>();
            container.Register<IDownloader, Downloader>();
            container.Register<IDatabaseManagementService, MsSqlDatabaseManagementService>();
            container.Register<IStockQuotesDownloadService, StockQuotesDownloadService>();
            container.Register<IStockQuotesMigrationFromCsv, StockQuotesMigrationFromCsv>();


            container.RegisterInstance(typeof(CultureInfo), CultureInfo.InvariantCulture);
            container.Register<ClassMap<StockQuote>, StockQuoteCsvClassMap>();
        }
    }
}
