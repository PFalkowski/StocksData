using LoggerLite;
using SimpleInjector;
using Stocks.Data.Api;
using Stocks.Data.Common.Models;
using Stocks.Data.ConsoleApp.Startup;
using System;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using static ConsoleUserInteractionHelper.ConsoleHelper;

namespace Stocks.Data.ConsoleApp
{
    class Program
    {
        static async Task Main(string[] args)
        {
            var configuration = new ConfigurationBuilder()
                .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
                .AddEnvironmentVariables()
                .AddCommandLine(args)
                .Build();
            
            await using var container = new Container();
            await using var scope = container.Initialize(configuration).BeginScopedLifestyle();

            var logger = container.GetInstance<ILogger>();
            var projectSettings = container.GetInstance<IProjectSettings>();
            var api = container.GetInstance<IStocksDataApi>();

            await RunUserLoop(logger, projectSettings, api);
        }

        private static async Task RunUserLoop(ILogger logger, IProjectSettings projectSettings, IStocksDataApi api)
        {
            logger.LogInfo($"Hello in {projectSettings.ProjectName}. Type \"h\" for help \"q\" to exit.");
            string userInput;

            while (!(userInput = GetNonEmptyStringFromUser()).Equals("q", StringComparison.OrdinalIgnoreCase))
            {
                try
                {
                    var splitted = userInput.Split(' ');
                    await api.Execute(splitted);
                }
                catch (Exception e)
                {
                    logger.LogError(e);
                }
            }
        }
    }
}