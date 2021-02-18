using CsvHelper.Configuration;
using LoggerLite;
using Microsoft.Extensions.Configuration;
using Services.IO;
using SimpleInjector;
using Stocks.Data.Ado;
using Stocks.Data.Api.Services;
using Stocks.Data.Infrastructure;
using Stocks.Data.Model;
using Stocks.Data.Services;
using System.Globalization;
using Microsoft.EntityFrameworkCore;
using SimpleInjector.Lifestyles;
using Stocks.Data.Common.Models;
using Stocks.Data.Ef;
using Stocks.Data.TradingSimulator;
using Stocks.Data.TradingSimulator.Models;

namespace Stocks.Data.ConsoleApp.Startup
{
    public static class SimpleInjectorInitialize
    {
        public static void Initialize(Container container, IConfiguration configuration)
        {
            container.Options.DefaultScopedLifestyle = new AsyncScopedLifestyle();

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

            container.Register<ITradingSimulationConfig, TradingSimulationConfig>();

            #region Decorators
            container.RegisterDecorator<IStockQuoteRepository, StockQuotesRepositoryCacheDecorator>(Lifestyle.Scoped);
            #endregion

            #region Singletons
            container.RegisterSingleton<IProjectSettings, ProjectSettings>();
            #endregion

            container.Register<DbContext, StockContext>(Lifestyle.Scoped);
            #region Repositories

            container.Register<ICompanyRepository, CompanyRepository>(Lifestyle.Scoped);
            container.Register<IStockQuoteRepository, StockQuoteRepository>(Lifestyle.Scoped);

            #endregion

            #region Trading Simulators

            container.Register<ITradingSimulator, Top10TradingSimulator>();

            #endregion

            container.RegisterInstance(typeof(CultureInfo), CultureInfo.InvariantCulture);
            container.Register<ClassMap<StockQuote>, StockQuoteCsvClassMap>();
        }
    }
}
