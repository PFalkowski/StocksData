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
using AutoMapper;
using Extensions.Standard;
using static ConsoleUserInteractionHelper.ConsoleHelper;

namespace Stocks.Data.ConsoleApp
{
    class Program
    {
        private const string HelpMessage = @"Usage: 
- download: download stock archive
- migrate: seed the database with data unzipped in working directory
- dropDb: Remove the database
- print: print selected stock quotes
- simulate: run trading simulation
- predict: get prediction for selected day
- cleanDir: cleans output directory
- unzip: unzip archive to output dir";
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
            var tradingSimulationConfig = Container.GetInstance<ITradingSimulationConfig>();

            await using var scope = AsyncScopedLifestyle.BeginScope(Container);
            using var companyRepository = Container.GetInstance<ICompanyRepository>();
            using var stockQuoteRepository = Container.GetInstance<IStockQuoteRepository>();
            using var tradingSimulationResultRepository = Container.GetInstance<ITradingSimulationResultRepository>();
            var simulator = Container.GetInstance<ITradingSimulator>();
            var mapper = Container.GetInstance<IMapper>();

            logger.LogInfo($"Hello in {projectSettings.Name}. Type \"h\" for help");
            string userInput;

            var progressReporter = new ConsoleProgressReporter();
            while (!(userInput = GetNonEmptyStringFromUser()).Equals("q", StringComparison.OrdinalIgnoreCase))
            {
                switch (userInput)
                {
                    case "download":
                        await quotesDownloader.Download(projectSettings);
                        break;
                    case "migrate":
                        await api.Migrate(projectSettings, TargetLocation.Directory);
                        logger.LogInfo("Successfully migrated data to database.");
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
                        logger.LogInfo("Enter starting date in format YYYY-MM-DD: ");
                        tradingSimulationConfig.FromDate = GetDateFromUser();
                        logger.LogInfo("Enter end date in format YYYY-MM-DD: ");
                        tradingSimulationConfig.ToDate = GetDateFromUser();
                        tradingSimulationConfig.StartingCash = 1000;
                        

                        var allQuotesPrefilterd = stockQuoteRepository
                            .GetAll(x => !projectSettings.BlackListPattern.IsMatch(x.Ticker)
                                         && x.DateParsed.InOpenRange(tradingSimulationConfig.FromDate.AddDays(-30), tradingSimulationConfig.ToDate))
                            .ToList();
                        var simulationResult = simulator.Simulate(allQuotesPrefilterd, tradingSimulationConfig, progressReporter);
                        logger.LogInfo(simulationResult.ToString());
                        break;
                    case "predict":
                        logger.LogInfo("Enter date in format YYYY-MM-DD: ");
                        var date = GetDateFromUser();
                        var prediction = simulator.GetSignals(tradingSimulationConfig, date);
                        logger.LogInfo($"Stonks to buy: {string.Join(", ", prediction.Select(x => x.Ticker).OrderBy(x => x))}. Used prediction made with data from session {prediction.Select(x => x.DateParsed).First()} and config = {tradingSimulationConfig}");
                        break;
                    case "cleanDir":
                        projectSettings.CleanOutputDirectory();
                        logger.LogInfo($"Cleaned directory {projectSettings.UnzippedFilesDirectory.FullName}");
                        break;
                    case "unzip":
                        await api.Unzip(projectSettings);
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
