using Extensions.Standard;
using LoggerLite;
using Services.IO;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Stocks.Data.Infrastructure;

namespace Stocks.Data.Services.TestHarness
{
    internal class Program
    {
        private static async Task Main(string[] args)
        {
            const string projectName = "Stocks.Data.Services.TestHarness";
            const string inputDirName = "Input";
            const string outputDirName = "Output";
            const string logFileName = "Stocks.Data.Services.TestHarness";
            var inputDirectory = new DirectoryInfo(Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments), projectName, inputDirName));
            var outputDirectory = new DirectoryInfo(Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments), projectName, outputDirName));
            const string url = "http://bossa.pl/pub/ciagle/mstock/mstcgl.zip";
            //const string url = "http://example.com";
            CultureInfo.CurrentCulture = CultureInfo.InvariantCulture;
            if (!outputDirectory.Exists) { outputDirectory.Create(); }
            if (!inputDirectory.Exists) { inputDirectory.Create(); }
            var logger = new AggregateLogger(new ConsoleLogger { InfoColor = ConsoleColor.Gray }, new FileLoggerBase(Path.Combine(outputDirectory.FullName, logFileName)));


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
                    await SaveArchive(rawBytes, outputDirectory, logger);
                }
            }
            else
            {
                logger.LogInfo("Would you like to read archive from disk? (y/n)");
                var readArchive = GetBinaryDecisionFromUser();
                if (readArchive)
                {
                    logger.LogInfo("Point to the archive: (e to exit)");
                    var userInput = GetPathFromUser();

                    if (!userInput.Equals("e", StringComparison.InvariantCultureIgnoreCase))
                    {
                        var fileReader = new FileService();
                        var taskToReadFile = fileReader.ReadAllBytesAsync(userInput);
                        var elapsed = ShowSpinnerUntilTaskIsRunning(taskToReadFile);
                        rawBytes = await taskToReadFile;
                        logger.LogInfo($"Read {rawBytes.Length} bytes in {elapsed.AsTime()}");
                    }
                }
            }

            Dictionary<string, string> unzippedStocks = null;
            if (rawBytes?.Length > 0)
            {
                Console.WriteLine();
                var unzipper = new Unzipper();
                var taskToUnzip = unzipper.UnzipAsync(rawBytes);
                var elapsed= ShowSpinnerUntilTaskIsRunning(taskToUnzip);
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
            }

            Console.WriteLine("press any key to exit...");
            Console.ReadKey();
        }

        private static string GetPathFromUser()
        {
            string userInput = Console.ReadLine();
            while (string.IsNullOrWhiteSpace(userInput) ||
                   (!File.Exists(userInput) || !userInput.EndsWith(".zip", StringComparison.InvariantCultureIgnoreCase)) &&
                   !userInput.Equals("e", StringComparison.InvariantCultureIgnoreCase))
            {
                userInput = Console.ReadLine();
            }

            return userInput;
        }

        private static async Task SaveArchive(byte[] rawBytes, DirectoryInfo outputDirectory, ILogger logger)
        {
            if (rawBytes == null) throw new ArgumentNullException(nameof(rawBytes));
            if (outputDirectory == null) throw new ArgumentNullException(nameof(outputDirectory));
            if (logger == null) throw new ArgumentNullException(nameof(logger));

            var fileName = Path.Combine(outputDirectory.FullName, "mstcgl.zip");
            var overwrite = false;
            var fileExists = File.Exists(fileName);
            if (fileExists)
            {
                logger.LogInfo($"File {fileName} already exists. Would you like to overwrite?");
                overwrite = GetBinaryDecisionFromUser();
            }

            if (!fileExists || overwrite)
            {
                var writeToFile = File.WriteAllBytesAsync(fileName, rawBytes);
                var elapsed = ShowSpinnerUntilTaskIsRunning(writeToFile);
                await writeToFile;
                logger.LogInfo($"archive saved to {fileName} in {elapsed.AsTime()}");
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

        public static TimeSpan ShowSpinnerUntilConditionTrue(Func<bool> condition)
        {
            if (condition == null) throw new ArgumentNullException(nameof(condition));

            var watch = Stopwatch.StartNew();
            var i = 0;
            Console.CursorVisible = false;
            while (condition.Invoke())
            {
                ClearCurrentConsoleLine();
                switch (i % 4)
                {
                    case 0:
                        Console.Write("[\\]");
                        break;
                    case 1:
                        Console.Write("[|]");
                        break;
                    case 2:
                        Console.Write("[/]");
                        break;
                    case 3:
                        Console.Write("[-]");
                        break;
                    default:
                        break;
                }

                Thread.Sleep(200);
                ++i;
            }
            watch.Stop();
            ClearCurrentConsoleLine();
            Console.CursorVisible = true;
            return watch.Elapsed;
        }

        public static TimeSpan ShowSpinnerUntilTaskIsRunning(Task task)
        {
            return ShowSpinnerUntilConditionTrue(() => !task.GetAwaiter().IsCompleted);
        }
        public static TimeSpan ShowSpinnerUntilTaskIsRunning<T>(Task<T> task)
        {
            return ShowSpinnerUntilConditionTrue(() => !task.GetAwaiter().IsCompleted);
        }

        public static void ClearCurrentConsoleLine()
        {
            var currentLineCursor = Console.CursorTop;
            Console.SetCursorPosition(0, Console.CursorTop);
            Console.Write(new string(' ', Console.BufferWidth));
            Console.SetCursorPosition(0, currentLineCursor);
        }
        public static bool GetBinaryDecisionFromUser()
        {
            bool? response = null;
            while (response == null)
            {
                var key = Console.ReadKey();
                switch (key.Key)
                {
                    case ConsoleKey.N:
                        response = false;
                        break;
                    case ConsoleKey.Y:
                        response = true;
                        break;
                    default:
                        Console.WriteLine($"Only key 'Y' or 'N' are acceptable. Provided invalid key \"{key.Key}\"");
                        break;
                }
            }
            return response.Value;
        }
    }
}
