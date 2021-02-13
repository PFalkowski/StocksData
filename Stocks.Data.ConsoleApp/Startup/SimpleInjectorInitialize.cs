﻿using CsvHelper.Configuration;
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
            
            #region Singletons
            container.RegisterSingleton<IProjectSettings, ProjectSettings>();
            #endregion

            container.Register<DbContext, StockContext>(Lifestyle.Scoped);
            #region Repositories

            container.Register<ICompanyRepository, CompanyRepository>(Lifestyle.Scoped);
            //container.Register<ICompanyRepository<StockQuote>, Repository<StockQuote>>();

            #endregion

            container.RegisterInstance(typeof(CultureInfo), CultureInfo.InvariantCulture);
            container.Register<ClassMap<StockQuote>, StockQuoteCsvClassMap>();
        }
    }
}
