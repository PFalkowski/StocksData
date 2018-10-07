﻿using Extensions.Standard;
using LoggerLite;
using Services.IO;
using System;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Threading;
using System.Threading.Tasks;

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
            //const string url = "http://bossa.pl/pub/ciagle/mstock/mstcgl.zip";
            const string url = "http://example.com";
            CultureInfo.CurrentCulture = CultureInfo.InvariantCulture;
            if (!outputDirectory.Exists) { outputDirectory.Create(); }
            if (!inputDirectory.Exists) { inputDirectory.Create(); }
            var logger = new AggregateLogger(new ConsoleLogger { InfoColor = ConsoleColor.Gray }, new FileLoggerBase(Path.Combine(outputDirectory.FullName, logFileName)));


            logger.LogInfo($"Hello in {projectName}. This test will read a directory and load it into Dictionary of deserialized stock quotes.");
            logger.LogInfo($"Would you like to download latest stocks from {url} ? (y/n)");
            var response = PromptUserForYesNo();

            if (response)
            {
                Console.WriteLine();
                var downloader = new Downloader();
                var watch = Stopwatch.StartNew();
                var rawBytes = downloader.GetBytesAsync(new Uri(url));
                ShowSpinnerUntilTaskIsRunning(rawBytes);
                //await rawBytes;
                logger.LogInfo($@"Downloaded {rawBytes.Result.Length} in {watch.ElapsedMilliseconds.AsTime()}");

                logger.LogInfo("Would you like to save archive to disk? (y/n)");
                var saveArchive = PromptUserForYesNo();
                if (saveArchive)
                {
                    var fileName = Path.Combine(outputDirectory.FullName, "mstcgl.zip");
                    var overwrite = false;
                    var fileExists = File.Exists(fileName);
                    if (fileExists)
                    {
                        logger.LogInfo($"File already exists. Would you like to overwrite?");
                        overwrite = PromptUserForYesNo();
                    }
                    if (!fileExists || overwrite)
                    {
                        var writeToFile = File.WriteAllBytesAsync(fileName, rawBytes.Result);
                        ShowSpinnerUntilTaskIsRunning(writeToFile);
                        //await writeToFile;
                        logger.LogInfo($"archive saved to {fileName}");
                    }
                }

                Console.WriteLine();
                var unzipper = new Unzipper();
            }

            Console.WriteLine("press any key to exit...");
            Console.ReadKey();
        }

        public static void ShowSpinnerUntilConditionTrue(Func<bool> condition)
        {
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

            ClearCurrentConsoleLine();
            Console.CursorVisible = true;
        }

        public static void ShowSpinnerUntilTaskIsRunning(Task task)
        {
            ShowSpinnerUntilConditionTrue(() => !task.GetAwaiter().IsCompleted);
        }
        public static void ShowSpinnerUntilTaskIsRunning<T>(Task<T> task)
        {
            ShowSpinnerUntilConditionTrue(() => !task.GetAwaiter().IsCompleted);
        }

        public static void ClearCurrentConsoleLine()
        {
            var currentLineCursor = Console.CursorTop;
            Console.SetCursorPosition(0, Console.CursorTop);
            Console.Write(new string(' ', Console.BufferWidth));
            Console.SetCursorPosition(0, currentLineCursor);
        }
        public static bool PromptUserForYesNo()
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
