using System;
using System.Globalization;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using LoggerLite;
using Services.IO;
using Stocks.Data.Common.Models;

namespace Stocks.Data.Services.Tier1
{
    public class StockQuotesDownloadService : IStockQuotesDownloadService
    {
        private readonly IDownloader _downloader;
        private readonly ILogger _logger;

        public StockQuotesDownloadService(IDownloader downloader, ILogger logger)
        {
            _downloader = downloader;
            _logger = logger;
        }

        public async Task Download(IProjectSettings project)
        {
            var workingDirectory = project.WorkingDirectory;
            CultureInfo.CurrentCulture = CultureInfo.InvariantCulture;
            if (!workingDirectory.Exists) { workingDirectory.Create(); }

            var rawBytes = await DownloadStocksArchive(project.QuotesDownloadUrl, _logger);

            await SaveArchive(rawBytes, new FileInfo(Path.Combine(workingDirectory.FullName, project.ArchiveFileName)), _logger);
        }

        private async Task SaveArchive(byte[] rawBytes, FileInfo outputFile, ILogger logger = null, bool overwrite = true)
        {
            if (rawBytes == null) throw new ArgumentNullException(nameof(rawBytes));
            if (outputFile == null) throw new ArgumentNullException(nameof(outputFile));

            var fileExists = outputFile.Exists;
            if (fileExists)
            {
                logger?.LogWarning($"File {outputFile.FullName} already exists. File will be overwritten.");
            }

            if (!fileExists || overwrite)
            {
                var writeToFile = File.WriteAllBytesAsync(outputFile.FullName, rawBytes);
                await writeToFile;
                logger?.LogInfo($"archive saved to {outputFile.FullName}.");
            }
        }

        private async Task<byte[]> DownloadStocksArchive(string url, ILogger logger)
        {
            if (url == null) throw new ArgumentNullException(nameof(url));
            if (logger == null) throw new ArgumentNullException(nameof(logger));
            
            var rawBytes = _downloader.GetBytesAsync(new Uri(url), CancellationToken.None);
            var res = await rawBytes;
            logger.LogInfo($"Downloaded {res.Length}");
            return res;
        }
    }
}
