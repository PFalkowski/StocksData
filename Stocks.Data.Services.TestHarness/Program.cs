using Extensions.Standard;
using LoggerLite;
using Services.IO;
using Stocks.Data.Infrastructure;
using Stocks.Data.Model;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using static ConsoleUserInteractionHelper.ConsoleHelper;

namespace Stocks.Data.Services.TestHarness
{
    internal class Program
    {
        private static async Task Main(string[] args)
        {
            const string projectName = "Stocks.Data.Services.TestHarness";
            const string outputDirName = "Output";
            const string logFileName = "Stocks.Data.Services.TestHarness";
            const string archiveFileName = "mstcgl.zip";
            const string unzippedFilesDirectoryName = "mstcgl";
            const string quotesFileExtension = "mst";
            var workingDirectory = new DirectoryInfo(Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments), projectName, outputDirName));
            const string url = "http://bossa.pl/pub/ciagle/mstock/mstcgl.zip";
            //const string url = "http://example.com";
            CultureInfo.CurrentCulture = CultureInfo.InvariantCulture;
            if (!workingDirectory.Exists) { workingDirectory.Create(); }
            var logger = new AggregateLogger(
                new ConsoleLogger { InfoColor = ConsoleColor.Gray, Formatter = (level, message) => $"{message}{Environment.NewLine}" }, 
                new FileLoggerBase(Path.Combine(workingDirectory.FullName, logFileName)));

            var unzippedFilesDirectory =
                new DirectoryInfo(Path.Combine(workingDirectory.FullName, unzippedFilesDirectoryName));

            Dictionary<string, string> unzippedStocks = null;
            logger.LogInfo($"Hello in {projectName}. This test will read a directory and load it into Dictionary of deserialized stock quotes.");
            logger.LogInfo($"Would you like to download latest stocks from {url} ? (y/n)");
            var response = GetBinaryDecisionFromUser();
            //var unzipArchive = false;
            byte[] rawBytes = null;
            if (response)
            {
                rawBytes = await DownloadStocksArchive(url, logger);

                logger.LogInfo("Would you like to save archive to disk? (y/n)");
                var saveArchive = GetBinaryDecisionFromUser();
                if (saveArchive)
                {
                    await SaveArchive(rawBytes, new FileInfo(Path.Combine(workingDirectory.FullName, archiveFileName)), logger);
                }
            }
            else
            {
                logger.LogInfo("Would you like to read archive from disk? (y/n)");
                var readArchive = GetBinaryDecisionFromUser();
                FileInfo inputFile = null;
                var useExistingFile = false;
                if (readArchive)
                {
                    if (File.Exists(Path.Combine(workingDirectory.FullName, archiveFileName)))
                    {
                        logger.LogInfo($"Would you like to use exisitng archive from {workingDirectory.FullName}? (y/n)");
                        useExistingFile = GetBinaryDecisionFromUser();
                        if (useExistingFile)
                        {
                            inputFile = new FileInfo(Path.Combine(workingDirectory.FullName, archiveFileName));
                        }
                    }
                    if (!useExistingFile)
                    {
                        logger.LogInfo("Point to the archive:");
                        var userInput = GetPathToExistingFileFromUser(".zip");
                        inputFile = new FileInfo(userInput);
                    }

                    var fileReader = new FileService();
                    var taskToReadFile = fileReader.ReadAllBytesAsync(inputFile.FullName);
                    var elapsed = ShowSpinnerUntilTaskIsRunning(taskToReadFile);
                    rawBytes = await taskToReadFile;
                    logger.LogInfo($"Read {rawBytes.Length} bytes in {elapsed.AsTime()}");
                }
                else
                {
                    logger.LogInfo("Would you like to read unzipped files from disk? (y/n)");
                    var readFiles = GetBinaryDecisionFromUser();
                    if (readFiles)
                    {
                        var useFilesFromDefaultDir = false;
                        if (unzippedFilesDirectory.Exists)
                        {
                            var filesFound = Directory.GetFiles(unzippedFilesDirectory.FullName).Select(f => new FileInfo(f))
                                .Count(f => f.Extension.EndsWith(quotesFileExtension));
                            if (filesFound > 0)
                            {
                                logger.LogInfo($"Would you like to use {filesFound} files from {workingDirectory.FullName}? (y/n)");
                                useFilesFromDefaultDir = GetBinaryDecisionFromUser();
                                //if (useFilesFromDefaultDir)
                                //{
                                //    unzippedFilesDirectory = new DirectoryInfo(Path.Combine(workingDirectory.FullName, unzippedFilesDirectoryName));
                                //}
                            }
                        }
                        if (!useFilesFromDefaultDir)
                        {
                            logger.LogInfo("Point to the directory with mst files:");
                            var userInput = GetPathToExistingDirectoryFromUser();
                            unzippedFilesDirectory = new DirectoryInfo(userInput.FullName);
                        }

                        var directoryStocksReader = new DirectoryService(new FileService());
                        var taskToRead = directoryStocksReader.ReadTopDirectoryAsync(unzippedFilesDirectory.FullName, $"*.{quotesFileExtension}");
                        var elapsed = ShowSpinnerUntilTaskIsRunning(taskToRead);
                        unzippedStocks = await taskToRead;
                        logger.LogInfo($"Read {unzippedStocks.Count} stocks in {elapsed.AsTime()}");
                    }
                }
            }

            if (rawBytes?.Length > 0)
            {
                Console.WriteLine();
                var unzipper = new Unzipper();
                var taskToUnzip = unzipper.UnzipAsync(rawBytes);
                var elapsed = ShowSpinnerUntilTaskIsRunning(taskToUnzip);
                unzippedStocks = await taskToUnzip;
                logger.LogInfo($"Unzipped {unzippedStocks.Count} stocks in {elapsed.AsTime()}");
            }

            if (unzippedStocks?.Count > 0)
            {
                logger.LogInfo($"Deserializing {unzippedStocks.Count} stocks:");
                var bullkDeserializer = new StocksBulkDeserializer(new StocksDeserializer(new StockQuoteCsvClassMap()));
                var taskToDeserialize = bullkDeserializer.DeserializeAsync(unzippedStocks);
                var elapsed = ShowSpinnerUntilTaskIsRunning(taskToDeserialize);
                var deserialized = await taskToDeserialize;

                logger.LogInfo($"Deserialzed {unzippedStocks.Count} stocks in {elapsed.AsTime()}");

                logger.LogInfo($"Would you like to print some stock quotes? (y/n)");
                var decision = GetBinaryDecisionFromUser();
                if (decision)
                {
                    logger.LogInfo("Please enter the TICKER for the stock of your choice:");
                    var line = GetNonEmptyStringFromUser();
                    Company found = null;
                    while ((found = deserialized.FirstOrDefault(c => c.Ticker.Equals(line, StringComparison.InvariantCultureIgnoreCase))) == null)
                    {
                        logger.LogInfo($"{line} not found in the collection.");
                        line = GetNonEmptyStringFromUser();
                    }
                    found.Quotes.ForEach(q => Console.Out.WriteLine($"{q} Open: {q.Open} High: {q.High} Low: {q.Low} Close: {q.Close}"));
                }
                logger.LogInfo($"Would you like save files to {unzippedFilesDirectory.FullName}? (y/n)");
                var saveFiles = GetBinaryDecisionFromUser();
                if (saveFiles)
                {
                    if (!unzippedFilesDirectory.Exists) { unzippedFilesDirectory.Create(); }
                    var tasksToSave = new List<Task>();
                    foreach (var stock in unzippedStocks)
                    {
                        tasksToSave.Add(File.WriteAllTextAsync(Path.Combine(unzippedFilesDirectory.FullName, stock.Key), stock.Value));
                    }
                    var taskForSave = Task.WhenAll(tasksToSave);
                    var elapsedWhenSaving = ShowSpinnerUntilTaskIsRunning(taskForSave);
                    logger.LogInfo($"Saved {unzippedStocks.Count} stocks to {unzippedFilesDirectory.FullName} in {elapsed.AsTime()}");
                }
            }

            Console.WriteLine("press any key to exit...");
            Console.ReadKey();
        }

        private static DirectoryInfo GetPathToExistingDirectoryFromUser()
        {
            var userInput = Console.ReadLine();
            var directoryExists = false;
            var inputEmpty = true;
            while ((inputEmpty = string.IsNullOrEmpty(userInput)) || (directoryExists = !Directory.Exists(userInput)))
            {
                if (inputEmpty)
                {
                    Console.WriteLine("Input cannot be empty. Please enter non-empty string:");
                }
                else
                {
                    Console.WriteLine("Provided directory does not exist. Please enter path to exisitng folder:");
                }
                userInput = Console.ReadLine();
            }

            return new DirectoryInfo(userInput);
        }

        private static async Task SaveArchive(byte[] rawBytes, FileInfo outputFile, ILogger logger)
        {
            if (rawBytes == null) throw new ArgumentNullException(nameof(rawBytes));
            if (outputFile == null) throw new ArgumentNullException(nameof(outputFile));
            if (logger == null) throw new ArgumentNullException(nameof(logger));

            var overwrite = false;
            var fileExists = outputFile.Exists;
            if (fileExists)
            {
                logger.LogInfo($"File {outputFile.FullName} already exists. Would you like to overwrite?");
                overwrite = GetBinaryDecisionFromUser();
            }

            if (!fileExists || overwrite)
            {
                var writeToFile = File.WriteAllBytesAsync(outputFile.FullName, rawBytes);
                var elapsed = ShowSpinnerUntilTaskIsRunning(writeToFile);
                await writeToFile;
                logger.LogInfo($"archive saved to {outputFile.FullName} in {elapsed.AsTime()}");
            }
        }

        private static async Task<byte[]> DownloadStocksArchive(string url, ILogger logger)
        {
            if (url == null) throw new ArgumentNullException(nameof(url));
            if (logger == null) throw new ArgumentNullException(nameof(logger));

            Console.WriteLine();
            var downloader = new Downloader();
            var rawBytes = downloader.GetBytesAsync(new Uri(url));
            var elapsed = ShowSpinnerUntilTaskIsRunning(rawBytes);
            var res = await rawBytes;
            logger.LogInfo($"Downloaded {res.Length} in {elapsed.AsTime()}");
            return res;
        }
    }
}
