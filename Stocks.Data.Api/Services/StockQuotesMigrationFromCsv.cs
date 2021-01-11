using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using LoggerLite;
using Services.IO;
using Stocks.Data.Ado;
using Stocks.Data.Api.Models;
using Stocks.Data.Model;
using Stocks.Data.Services;

namespace Stocks.Data.Api.Services
{
    public class StockQuotesMigrationFromCsv : IStockQuotesMigrationFromCsv
    {
        private readonly ILogger _logger;
        private readonly IDirectoryService _directoryStocksReader;
        private readonly IUnzipper _unzipper;
        private readonly IStocksBulkDeserializer _stocksBulkDeserializer;
        private readonly IDatabaseManagementService _databaseManagementService;
        private readonly CompanyBulkInserter _companyBulkInserter;

        public StockQuotesMigrationFromCsv(ILogger logger,
            IDirectoryService directoryStocksReader,
            IUnzipper unzipper,
            IStocksBulkDeserializer stocksBulkDeserializer,
            IDatabaseManagementService databaseManagementService,
            CompanyBulkInserter companyBulkInserter)
        {
            _logger = logger;
            _directoryStocksReader = directoryStocksReader;
            _unzipper = unzipper;
            _stocksBulkDeserializer = stocksBulkDeserializer;
            _databaseManagementService = databaseManagementService;
            _companyBulkInserter = companyBulkInserter;
        }

        public async Task Migrate(Project project, TargetLocation location)
        {
            Dictionary<string, string> fromDisk;
            switch (location)
            {
                case TargetLocation.File:
                    fromDisk = await ReadFromDisk(project);
                    break;
                case TargetLocation.ZipArchive:
                    fromDisk = await ReadFromZip(project);
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(location), location, null);
            }
            var deserialized = await _stocksBulkDeserializer.DeserializeAsync(fromDisk);

            await _databaseManagementService.EnsureDbExists(project);
            _companyBulkInserter.BulkInsert(project.ConnectionString, nameof(Company), deserialized);
        }


        private async Task<Dictionary<string, string>> ReadFromZip(Project project)
        {
            if (!project.ArchiveFile.Exists)
            {
                throw new FileNotFoundException(project.ArchiveFile.FullName);
            }

            var fileReader = new FileService();
            var rawBytes = await fileReader.ReadAllBytesAsync(project.ArchiveFile.FullName);
            _logger.LogInfo($"Read {rawBytes.Length} bytes from archive.");

            var unzippedStocks = await _unzipper.UnzipAsync(rawBytes, CancellationToken.None);
            _logger.LogInfo($"Unzipped {unzippedStocks.Count} stocks from {project.ArchiveFileName}");

            return unzippedStocks;
        }

        private async Task<Dictionary<string, string>> ReadFromDisk(Project project)
        {
            if (!project.ArchiveFile.Exists)
            {
                throw new FileNotFoundException(project.ArchiveFile.FullName);
            }

            var filesFound = Directory.GetFiles(project.UnzippedFilesDirectory.FullName).Select(f => new FileInfo(f))
                .Count(f => f.Extension.EndsWith(project.QuotesFileExtension));

            if (filesFound == 0)
            {
                _logger.LogError($"Found no files in {project.WorkingDirectory.FullName} with extension {project.QuotesFileExtension}.");
                throw new ArgumentException(project.ArchiveFile.FullName);
            }

            _logger.LogInfo($"Found {filesFound} files in {project.WorkingDirectory.FullName}.");


            var unzippedStocks = await _directoryStocksReader.ReadTopDirectoryAsync(project.UnzippedFilesDirectory.FullName, $"*.{project.QuotesFileExtension}");

            _logger.LogInfo($"Read {unzippedStocks.Count} stocks from {project.UnzippedFilesDirectoryName}");

            return unzippedStocks;
        }
    }
}
