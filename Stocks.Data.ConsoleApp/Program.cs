using LoggerLite;
using SimpleInjector;
using Stocks.Data.Api.Models;
using Stocks.Data.Api.Services;
using Stocks.Data.ConsoleApp.Startup;
using System.Linq;
using System.Threading.Tasks;
using static ConsoleUserInteractionHelper.ConsoleHelper;

namespace Stocks.Data.ConsoleApp
{
    class Program
    {
        private static Container Container { get; set; } = new Container();
        static async Task Main(string[] args)
        {
            SimpleInjectorInitialize.Initialize(Container, null);
            var projectSettings = ParseSettings(args);
            var api = Container.GetInstance<IStockQuotesMigrationFromCsv>();
            var quotesDownloader = Container.GetInstance<IStockQuotesDownloadService>();
            var dbManagementSvc = Container.GetInstance<IDatabaseManagementService>();
            var logger = Container.GetInstance<ILogger>();

            logger.LogInfo($"Hello in {projectSettings.Name}.");
            logger.LogInfo($"Would you like to download latest stocks from {projectSettings.QuotesDownloadUrl} ? (y/n)");
            var response = GetBinaryDecisionFromUser();

            if (response)
            {
                await quotesDownloader.Download(projectSettings);
            }

            logger.LogInfo("Would you like to upload quotes to database? (y/n)");
            response = GetBinaryDecisionFromUser();

            if (response)
            {
                await api.Migrate(projectSettings);
            }

        }

        private static ProjectSettings ParseSettings(string[] args)
        {
            var projectSettings = new ProjectSettings();
            foreach (var arg in args)
            {
                var split = arg.TrimStart('-').Split("-");
                var firstPart = split.FirstOrDefault();
                var secondPart = split.LastOrDefault();
                if (firstPart != null && secondPart != null &&
                    projectSettings.SettingsDictionary.ContainsKey(firstPart))
                {
                    projectSettings.SettingsDictionary[firstPart] = secondPart;
                }
            }

            return projectSettings;
        }
    }
}
