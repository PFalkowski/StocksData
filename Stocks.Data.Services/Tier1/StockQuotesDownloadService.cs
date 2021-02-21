using System;
using System.Globalization;
using System.IO;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Flurl;
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

            var rawBytes = await Download(project.QuotesDownloadUrl, _logger);

            await SaveArchive(rawBytes, new FileInfo(Path.Combine(workingDirectory.FullName, project.ArchiveFileName)), _logger);
        }

        public async Task<string> DownloadUpdate(IProjectSettings project, DateTime date)
        {
            var workingDirectory = project.WorkingDirectory;
            CultureInfo.CurrentCulture = CultureInfo.InvariantCulture;
            if (!workingDirectory.Exists) { workingDirectory.Create(); }

            var url = Url.Combine(project.QuotesUpdateUrlBase, $"{date:yyyyMMdd}.prn"); 
            var rawBytes = await Download(url, _logger);

            return Encoding.UTF8.GetString(rawBytes);
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

        private async Task<byte[]> Download(string url, ILogger logger)
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
