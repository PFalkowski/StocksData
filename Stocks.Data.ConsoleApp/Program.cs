using LoggerLite;
using SimpleInjector;
using Stocks.Data.Api.Services;
using Stocks.Data.Common.Models;
using Stocks.Data.ConsoleApp.Startup;
using Stocks.Data.Ef;
using System;
using System.Threading.Tasks;
using SimpleInjector.Lifestyles;
using static ConsoleUserInteractionHelper.ConsoleHelper;

namespace Stocks.Data.ConsoleApp
{
    class Program
    {
        private const string HelpMessage = @"Usage: 
- download: download stock archive and unzip it to working directory
- migrate: seed the database with data unzipped in working directory
- dopDb: Remove the database
- print: print selected stock quotes";
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

            await using var scope = AsyncScopedLifestyle.BeginScope(Container);
            using var companyRepository = Container.GetInstance<ICompanyRepository>();

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
                        await dbManagementSvc.EnsureDbDoesNotExist(projectSettings);
                        await api.Migrate(projectSettings);
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
                            found.Quotes.ForEach(q => Console.Out.WriteLine($"{q} Open: {q.Open} High: {q.High} Low: {q.Low} Close: {q.Close}"));
                        }
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
