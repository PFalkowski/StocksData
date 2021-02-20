using AutoMapper;
using ConsoleUserInteractionHelper;
using CsvHelper.Configuration;
using LoggerLite;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using ProgressReporting;
using Services.IO;
using SimpleInjector;
using SimpleInjector.Lifestyles;
using Stocks.Data.Ado;
using Stocks.Data.Api;
using Stocks.Data.Common;
using Stocks.Data.Common.Models;
using Stocks.Data.Ef;
using Stocks.Data.Model;
using Stocks.Data.Services.Tier0;
using Stocks.Data.Services.Tier1;
using Stocks.Data.TradingSimulator;
using Stocks.Data.TradingSimulator.Mapping;
using Stocks.Data.TradingSimulator.Models;
using System;
using System.Globalization;

namespace Stocks.Data.ConsoleApp.Startup
{
    public static class SimpleInjectorInitialize
    {
        public static Container Initialize(this Container container, IConfiguration configuration)
        {
            container.Options.DefaultScopedLifestyle = new AsyncScopedLifestyle();
            container.RegisterInstance(typeof(IConfiguration), configuration);
            container.RegisterInstance(typeof(ILogger), new AggregateLogger(new ConsoleLogger(), new FileLoggerBase($"log {DateTime.Now:yyyy-MM-dd HH-mm-ss}.txt")));

            #region Services
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
            #endregion

            #region Configuration
            container.RegisterInstance(typeof(CultureInfo), CultureInfo.InvariantCulture);
            container.Register<ClassMap<StockQuote>, StockQuoteCsvClassMap>();
            container.Register<IProjectSettings, ProjectSettings>();
            container.Register<ITradingSimulationConfig, TradingSimulationConfig>();
            var mapperConfig = new MapperConfiguration(cfg =>
            {
                cfg.AddProfile<TradingSimulatorAutoMapperProfile>();
            });
            mapperConfig.AssertConfigurationIsValid();
            container.RegisterSingleton<IMapper>(() => new Mapper(mapperConfig));

            container.RegisterSingleton<IProgressReportable, ConsoleProgressReporter>();
            #endregion

            #region Decorators
            container.RegisterDecorator<IStockQuoteRepository, StockQuotesRepositoryCacheDecorator>(Lifestyle.Scoped);
            #endregion

            #region DataAccess
            container.Register<DbContext, StockContext>(Lifestyle.Scoped);
            container.Register<ICompanyRepository, CompanyRepository>(Lifestyle.Scoped);
            container.Register<IStockQuoteRepository, StockQuoteRepository>(Lifestyle.Scoped);
            container.Register<ITradingSimulationResultRepository, TradingSimulationResultRepository>(Lifestyle.Scoped);
            #endregion

            #region Trading Simulators
            container.Register<ITradingSimulator, Top10TradingSimulator>();
            #endregion

            #region Api
            container.Register<IStocksDataApi, StocksDataApi>();
            #endregion

            container.Verify();

            return container;
        }

        public static Scope BeginScopedLifestyle(this Container container)
        {
            return AsyncScopedLifestyle.BeginScope(container);
        }
    }
}
