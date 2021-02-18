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
using Stocks.Data.Common.Models;
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
        private readonly BulkInserter<Company> _companyBulkInserter;

        public StockQuotesMigrationFromCsv(ILogger logger,
            IDirectoryService directoryStocksReader,
            IUnzipper unzipper,
            IStocksBulkDeserializer stocksBulkDeserializer,
            IDatabaseManagementService databaseManagementService,
            BulkInserter<Company> companyBulkInserter)
        {
            _logger = logger;
            _directoryStocksReader = directoryStocksReader;
            _unzipper = unzipper;
            _stocksBulkDeserializer = stocksBulkDeserializer;
            _databaseManagementService = databaseManagementService;
            _companyBulkInserter = companyBulkInserter;
        }

        public async Task Migrate(IProjectSettings project, TargetLocation location)
        {
            project.EnsureAllDirectoriesExist();
            Dictionary<string, string> fromDisk;
            switch (location)
            {
                case TargetLocation.Directory:
                    fromDisk = await ReadFromDirectory(project);
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

        public async Task Unzip(IProjectSettings project)
        {
            await ReadFromZip(project);
        }

        private async Task<Dictionary<string, string>> ReadFromZip(IProjectSettings project)
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

            await SaveUnzippedFiles(project, unzippedStocks);

            return unzippedStocks;
        }

        private async Task SaveUnzippedFiles(IProjectSettings project, Dictionary<string, string> unzippedStocks)
        {
            if (!project.UnzippedFilesDirectory.Exists) { project.UnzippedFilesDirectory.Create(); }
            var tasksToSave = new List<Task>();
            foreach (var stock in unzippedStocks)
            {
                if (!string.IsNullOrWhiteSpace(project.BlacklistPatternString) && project.BlackListPattern.IsMatch(stock.Key))
                {
                    _logger.LogWarning($"{stock.Key} matched blacklist. Skipping");
                    continue;
                }
                var path = Path.Combine(project.UnzippedFilesDirectory.FullName, stock.Key);
                if (File.Exists(path))
                {
                    _logger.LogWarning($"File {path} already exists and will be overwritten.");
                }
                tasksToSave.Add(File.WriteAllTextAsync(path, stock.Value));
            }
            _logger.LogInfo($"Saved {unzippedStocks.Count} files to {project.UnzippedFilesDirectory.FullName}.");

            await Task.WhenAll(tasksToSave);
        }

        private async Task<Dictionary<string, string>> ReadFromDirectory(IProjectSettings project)
        {
            if (!project.UnzippedFilesDirectory.Exists)
            {
                throw new DirectoryNotFoundException(project.UnzippedFilesDirectory.FullName);
            }

            var filesFound = Directory.GetFiles(project.UnzippedFilesDirectory.FullName).Select(f => new FileInfo(f))
                .Count(f => f.Extension.EndsWith(project.QuotesFileExtension));

            if (filesFound == 0)
            {
                _logger.LogError($"Found no files in {project.WorkingDirectory.FullName} with extension {project.QuotesFileExtension}.");
                throw new ArgumentException(project.ArchiveFile.FullName);
            }

            _logger.LogInfo($"Found {filesFound} files in {project.UnzippedFilesDirectory.FullName}.");


            var unzippedStocks = await _directoryStocksReader.ReadTopDirectoryAsync(project.UnzippedFilesDirectory.FullName, $"*.{project.QuotesFileExtension}");

            _logger.LogInfo($"Read {unzippedStocks.Count} stocks from {project.UnzippedFilesDirectoryName}");

            return unzippedStocks;
        }
    }
}
