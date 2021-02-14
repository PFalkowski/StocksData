using Extensions.Standard;
using LoggerLite;
using SimpleInjector;
using SimpleInjector.Lifestyles;
using Stocks.Data.Api.Services;
using Stocks.Data.Common.Models;
using Stocks.Data.ConsoleApp.Startup;
using Stocks.Data.Ef;
using Stocks.Data.TradingSimulator;
using Stocks.Data.TradingSimulator.Models;
using System;
using System.Linq;
using System.Threading.Tasks;
using static ConsoleUserInteractionHelper.ConsoleHelper;

namespace Stocks.Data.ConsoleApp
{
    class Program
    {
        private const string HelpMessage = @"Usage: 
- download: download stock archive and unzip it to working directory
- migrate: seed the database with data unzipped in working directory
- dopDb: Remove the database
- print: print selected stock quotes
- simulate: run trading simulation
- predict: get prediction for selected day";
        private static Container Container { get; set; } = new Container();
        static async Task Main(string[] args)
        {
            SimpleInjectorInitialize.Initialize(Container, null);
            var projectSettings = Container.GetInstance<IProjectSettings>();
            projectSettings.ParseSettings(args);
            var api = Container.GetInstance<IStockQuotesMigrationFromCsv>();
            var quotesDownloader = Container.GetInstance<IStockQuotesDownloadService>();
            var dbManagementSvc = Container.GetInstance<IDatabaseManagementService>();
            var logger = Container.GetInstance<ILogger>();
            var tradingConfig = Container.GetInstance<ITradingSimulationConfig>();

            await using var scope = AsyncScopedLifestyle.BeginScope(Container);
            using var companyRepository = Container.GetInstance<ICompanyRepository>();
            using var stockQuoteRepository = Container.GetInstance<IStockQuoteRepository>();
            var simulator = Container.GetInstance<ITradingSimulator>();

            logger.LogInfo($"Hello in {projectSettings.Name}. Type \"h\" for help");
            string userInput;

            while (!(userInput = GetNonEmptyStringFromUser()).Equals("q", StringComparison.OrdinalIgnoreCase))
            {
                switch (userInput)
                {
                    case "download":
                        await quotesDownloader.Download(projectSettings);
                        break;
                    case "migrate":
                        await api.Migrate(projectSettings, TargetLocation.ZipArchive);
                        break;
                    case "dropDb":
                        await dbManagementSvc.EnsureDbDoesNotExist(projectSettings);
                        break;
                    case "print":
                        logger.LogInfo("Please enter the TICKER for the stock of your choice:");
                        var line = GetNonEmptyStringFromUser();
                        var found = companyRepository.GetById(line);
                        if (found == null)
                        {
                            logger.LogWarning($"{line} not found.");
                        }
                        else
                        {
                            found.Quotes.ForEach(q => Console.Out.WriteLine(q.Summary()));
                        }
                        break;
                    case "simulate":
                        var progressReporter = new ConsoleProgressReporter();
                        tradingConfig.FromDate = new DateTime(2016, 01, 01);
                        tradingConfig.ToDate = new DateTime(2021, 02, 12);
                        tradingConfig.StartingCash = 1000;
                        var simulationResult = simulator.Simulate(tradingConfig, progressReporter);
                        logger.LogInfo(simulationResult.ToString());
                        break;
                    case "predict":
                        logger.LogInfo("Enter date in format YYYY-MM-DD: ");
                        var date = GetDateFromUser();
                        var quotes = stockQuoteRepository.GetAll(x =>
                            !tradingConfig.BlackListPattern.IsMatch(x.Ticker)
                            && x.DateParsed.InOpenRange(date.AddDays(-30), date.AddDays(30))).ToList();
                        var prediction = simulator.GetTopN(quotes, date);
                        logger.LogInfo($"Stonks to buy: {string.Join(", ", prediction.Select(x => x.Ticker))}. Used prediction made with data from session {prediction.Select(x => x.DateParsed).First()}");
                        break;
                    case "h":
                        logger.LogInfo(HelpMessage);
                        break;
                    default:
                        logger.LogInfo(@$"{userInput} not recognized as valid command. {HelpMessage}");
                        break;
                }
            }
        }
    }
}
