using AutoMapper;
using Extensions.Standard;
using LoggerLite;
using ProgressReporting;
using Stocks.Data.Common.Models;
using Stocks.Data.Ef;
using Stocks.Data.Model;
using Stocks.Data.TradingSimulator;
using Stocks.Data.TradingSimulator.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Stocks.Data.Services.Tier1;

namespace Stocks.Data.Api
{
    public class StocksDataApi : IStocksDataApi
    {
        private readonly IProjectSettings _projectSettings;
        private readonly IStockQuotesMigrationFromCsv _stockQuotesMigrationFromCsv;
        private readonly IStockQuotesDownloadService _stockQuotesDownloadService;
        private readonly IDatabaseManagementService _databaseManagementService;
        private readonly ILogger _logger;
        private readonly ITradingSimulationConfig _tradingSimulationConfig;
        private readonly ICompanyRepository _companyRepository;
        private readonly IStockQuoteRepository _stockQuoteRepository;
        private readonly ITradingSimulator _tradingSimulator;
        private readonly IProgressReportable _progressReporter;

        public StocksDataApi(
            IStockQuoteRepository stockQuoteRepository,
            ICompanyRepository companyRepository,
            ITradingSimulationConfig tradingSimulationConfig,
            ILogger logger,
            IDatabaseManagementService databaseManagementService,
            IStockQuotesDownloadService stockQuotesDownloadService,
            IStockQuotesMigrationFromCsv stockQuotesMigrationFromCsv,
            IProjectSettings projectSettings,
            ITradingSimulator tradingSimulator,
            IProgressReportable progressReporter)
        {
            _stockQuoteRepository = stockQuoteRepository;
            _companyRepository = companyRepository;
            _tradingSimulationConfig = tradingSimulationConfig;
            _logger = logger;
            _databaseManagementService = databaseManagementService;
            _stockQuotesDownloadService = stockQuotesDownloadService;
            _stockQuotesMigrationFromCsv = stockQuotesMigrationFromCsv;
            _projectSettings = projectSettings;
            _tradingSimulator = tradingSimulator;
            _progressReporter = progressReporter;
        }

        public async Task Execute(params string[] args)
        {
            var command = args[0];
            var fromDate = default(DateTime?);
            var toDate = default(DateTime?);
            switch (command)
            {
                case "h":
                case "help":
                    _logger.LogInfo(HelpMessage);
                    break;

                case "cleanDir":
                    _projectSettings.CleanOutputDirectory();
                    _logger.LogInfo($"Cleaned directory {_projectSettings.UnzippedFilesDirectory.FullName}");
                    break;

                case "dropDb":
                    await _databaseManagementService.EnsureDbDoesNotExist(_projectSettings);
                    break;

                case "download":
                    await _stockQuotesDownloadService.Download(_projectSettings);
                    break;

                case "unzip":
                    await _stockQuotesMigrationFromCsv.Unzip(_projectSettings);
                    break;

                case "migrate":
                    await _stockQuotesMigrationFromCsv.Migrate(_projectSettings, TargetLocation.Directory);
                    _logger.LogInfo("Successfully migrated data to database.");
                    break;

                case "print":
                    var found = _companyRepository.GetById(args[1]);
                    if (found == null)
                    {
                        _logger.LogWarning($"{args[1]} not found.");
                    }
                    else
                    {
                        _logger.LogInfo(string.Join(Environment.NewLine, found.Quotes.Select(x => x.Summary())));
                    }
                    break;

                case "simulate":
                    fromDate = TryParseDateTime(args[1]);
                    toDate = TryParseDateTime(args[2]);


                    if (fromDate.HasValue && toDate.HasValue)
                    {
                        _tradingSimulationConfig.FromDate = fromDate.Value;
                        _tradingSimulationConfig.ToDate = toDate.Value;
                    }

                    var allQuotesPrefilterd = _stockQuoteRepository
                        .GetAll(x => !_projectSettings.BlackListPattern.IsMatch(x.Ticker)
                                     && x.DateParsed.InOpenRange(_tradingSimulationConfig.FromDate.AddDays(-30), _tradingSimulationConfig.ToDate))
                        .ToList();
                    var simulationResult = _tradingSimulator.Simulate(allQuotesPrefilterd, _tradingSimulationConfig, _progressReporter);
                    _logger.LogInfo(simulationResult.ToString());
                    break;

                case "predict":
                    var date = TryParseDateTime(args[1]);
                    if (date.HasValue)
                    {
                        var prediction = _tradingSimulator.GetSignals(_tradingSimulationConfig, date.Value);
                        _logger.LogInfo($"Stonks to buy: {string.Join(", ", prediction.Select(x => x.Ticker).OrderBy(x => x))}. Used prediction made with data from session {prediction.Select(x => x.DateParsed).First()} and config = {_tradingSimulationConfig}");
                    }
                    else
                    {
                        _logger.LogError($"{args[1]} is not a valid date. Enter date in format YYYY-MM-DD");
                    }
                    break;

                default:
                    _logger.LogWarning(@$"{command} not recognized as valid command. {HelpMessage}");
                    break;
            }
        }

        private DateTime? TryParseDateTime(string input)
        {
            var result = DateTime.TryParse(input, out var date);

            if (result)
            {
                return date;
            }

            return null;
        }

        private const string HelpMessage = @"Usage: 
- download: download stock archive
- migrate: seed the database with data unzipped in working directory
- dropDb: Remove the database
- print: print selected stock quotes
- simulate: run trading simulation
- predict: get prediction for selected day
- cleanDir: cleans output directory
- unzip: unzip archive to output dir";
    }
}
