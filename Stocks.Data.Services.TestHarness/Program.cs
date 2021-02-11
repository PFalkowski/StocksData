using Extensions.Standard;
using LoggerLite;
using Services.IO;
using Stocks.Data.Api.Models;
using Stocks.Data.Api.Services;
using Stocks.Data.Infrastructure;
using Stocks.Data.Model;
using System;
using System.Collections.Generic;
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
            var project = new ProjectSettings();
            var logger = new AggregateLogger(
                new ConsoleLogger { InfoColor = ConsoleColor.Gray, Formatter = (level, message) => $"{message}{Environment.NewLine}" },
                new FileLoggerBase(Path.Combine(project.WorkingDirectory.FullName, project.LogFileName)));
            var quotesDownloader = new StockQuotesDownloadService(new Downloader(), logger);

            Dictionary<string, string> unzippedStocks = null;
            logger.LogInfo($"Hello in {project.Name}. This test will read a directory and load it into Dictionary of deserialized stock quotes.");
            logger.LogInfo($"Would you like to download latest stocks from {project.QuotesDownloadUrl} ? (y/n)");
            var response = GetBinaryDecisionFromUser();

            byte[] rawBytes = null;
            if (response)
            {
                await quotesDownloader.Download(project);
            }
            else
            {
                logger.LogInfo("Would you like to read archive from disk? (y/n)");
                var readArchive = GetBinaryDecisionFromUser();
                FileInfo inputFile = null;
                var useExistingFile = false;
                if (readArchive)
                {
                    if (File.Exists(Path.Combine(project.WorkingDirectory.FullName, project.ArchiveFileName)))
                    {
                        logger.LogInfo($"Would you like to use existing archive from {project.WorkingDirectory.FullName}? (y/n)");
                        useExistingFile = GetBinaryDecisionFromUser();
                        if (useExistingFile)
                        {
                            inputFile = new FileInfo(Path.Combine(project.WorkingDirectory.FullName, project.ArchiveFileName));
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
                        if (project.UnzippedFilesDirectory.Exists)
                        {
                            var filesFound = Directory.GetFiles(project.UnzippedFilesDirectory.FullName).Select(f => new FileInfo(f))
                                .Count(f => f.Extension.EndsWith(project.QuotesFileExtension));
                            if (filesFound > 0)
                            {
                                logger.LogInfo($"Would you like to use {filesFound} files from {project.WorkingDirectory.FullName}? (y/n)");
                                useFilesFromDefaultDir = GetBinaryDecisionFromUser();
                                //if (useFilesFromDefaultDir)
                                //{
                                //    unzippedFilesDirectory = new DirectoryInfo(Path.Combine(workingDirectory.FullName, unzippedFilesDirectoryName));
                                //}
                            }
                        }
                        if (!useFilesFromDefaultDir)
                        {
                            logger.LogInfo("Not supported. Bye!");
                        }

                        var directoryStocksReader = new DirectoryService(new FileService());
                        var taskToRead = directoryStocksReader.ReadTopDirectoryAsync(project.UnzippedFilesDirectory.FullName, $"*.{project.QuotesFileExtension}");
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
                logger.LogInfo($"Would you like save files to {project.UnzippedFilesDirectory.FullName}? (y/n)");
                var saveFiles = GetBinaryDecisionFromUser();
                if (saveFiles)
                {
                    if (!project.UnzippedFilesDirectory.Exists) { project.UnzippedFilesDirectory.Create(); }
                    var tasksToSave = new List<Task>();
                    foreach (var stock in unzippedStocks)
                    {
                        tasksToSave.Add(File.WriteAllTextAsync(Path.Combine(project.UnzippedFilesDirectory.FullName, stock.Key), stock.Value));
                    }
                    var taskForSave = Task.WhenAll(tasksToSave);
                    var elapsedWhenSaving = ShowSpinnerUntilTaskIsRunning(taskForSave);
                    logger.LogInfo($"Saved {unzippedStocks.Count} stocks to {project.UnzippedFilesDirectory.FullName} in {elapsed.AsTime()}");
                }
            }

            Console.WriteLine("press any key to exit...");
            Console.ReadKey();
        }
    }
}
