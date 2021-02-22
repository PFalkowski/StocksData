using AutoMapper;
using Extensions.Standard;
using LoggerLite;
using ProgressReporting;
using Stocks.Data.Common.Models;
using Stocks.Data.Ef;
using Stocks.Data.Services.Tier1;
using Stocks.Data.TradingSimulator;
using Stocks.Data.TradingSimulator.Models;
using System;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;

namespace Stocks.Data.Api
{
    public class StocksDataApi : IStocksDataApi
    {
        private readonly ILogger _logger;
        private readonly IProjectSettings _projectSettings;
        private readonly IStockQuotesMigrationFromCsv _stockQuotesMigrationFromCsv;
        private readonly IStockQuotesDownloadService _stockQuotesDownloadService;
        private readonly IDatabaseManagementService _databaseManagementService;
        private readonly ICompanyRepository _companyRepository;
        private readonly IStockQuoteRepository _stockQuoteRepository;
        private readonly ITradingSimulator _tradingSimulator;
        private readonly ITradingSimulationResultRepository _tradingSimulationResultRepository;
        private readonly IMapper _mapper;
        private readonly IProgressReportable _progressReporter;
        private readonly Microsoft.Extensions.Configuration.IConfiguration _configuration;
        private readonly IStockUpdateService _stockUpdateService;

        public StocksDataApi(IMapper mapper,
            ITradingSimulationResultRepository tradingSimulationResultRepository,
            IStockQuoteRepository stockQuoteRepository,
            ICompanyRepository companyRepository,
            ILogger logger,
            IDatabaseManagementService databaseManagementService,
            IStockQuotesDownloadService stockQuotesDownloadService,
            IStockQuotesMigrationFromCsv stockQuotesMigrationFromCsv,
            IProjectSettings projectSettings,
            ITradingSimulator tradingSimulator,
            IProgressReportable progressReporter,
            Microsoft.Extensions.Configuration.IConfiguration configuration,
            IStockUpdateService stockUpdateService)
        {
            _mapper = mapper;
            _tradingSimulationResultRepository = tradingSimulationResultRepository;
            _stockQuoteRepository = stockQuoteRepository;
            _companyRepository = companyRepository;
            _logger = logger;
            _databaseManagementService = databaseManagementService;
            _stockQuotesDownloadService = stockQuotesDownloadService;
            _stockQuotesMigrationFromCsv = stockQuotesMigrationFromCsv;
            _projectSettings = projectSettings;
            _tradingSimulator = tradingSimulator;
            _progressReporter = progressReporter;
            _configuration = configuration;
            _stockUpdateService = stockUpdateService;
        }

        public async Task Execute(params string[] args)
        {
            _projectSettings.EnsureAllDirectoriesExist();
            var command = args[0];
            var fromDate = default(DateTime?);
            var toDate = default(DateTime?);
            var tradingSimulationConfig = TradingSimulationConfig.CreateFrom(_configuration);
            switch (command)
            {
                case "h":
                case "help":
                    _logger.LogInfo(HelpMessage);
                    break;

                case "getDir":
                    _logger.LogInfo($"Working directory: {_projectSettings.WorkingDirectory.FullName}");
                    break;

                case "openDir":
                    ProcessStartInfo startInfo = new ProcessStartInfo ("explorer.exe", _projectSettings.WorkingDirectory.FullName);
                    startInfo.Verb = "runas";
                    Process.Start(startInfo);
                    break;

                case "printUnzipped":
                    _logger.LogInfo(string.Join(Environment.NewLine, _projectSettings.GetFilesListInDirectory(_projectSettings.UnzippedFilesDirectory)));
                    break;

                    
                case "cleanDir":
                    _projectSettings.CleanOutputDirectory();
                    _logger.LogInfo($"Cleaned directory {_projectSettings.UnzippedFilesDirectory.FullName}");
                    break;

                case "cleanLogs":
                    _projectSettings.CleanLogs();
                    _logger.LogInfo($"Cleaned logs in {_projectSettings.LogDirectory.FullName}");
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

                case "update":
                    await _stockUpdateService.PerformUpdateTillToday();
                    break;

                case "print":
                    if (args.Length >= 2)
                    {
                        var found = _companyRepository.GetById(args[1]);
                        if (found == null)
                        {
                            _logger.LogWarning($"{args[1]} not found.");
                        }
                        else
                        {
                            _logger.LogInfo(string.Join(Environment.NewLine, found.Quotes.Select(x => x.Summary())));
                        }
                    }
                    else
                    {
                        var allTickers = _companyRepository.GetAll().Select(x => x.Ticker).ToList();
                        _logger.LogInfo($"Found {allTickers.Count} companies.");
                        _logger.LogInfo(string.Join(Environment.NewLine, allTickers));
                    }
                    break;

                case "simulate":
                    if (args.Length >= 3)
                    {
                        fromDate = TryParseDateTime(args[1]);
                        toDate = TryParseDateTime(args[2]);
                        if (fromDate.HasValue && toDate.HasValue)
                        {
                            tradingSimulationConfig.FromDate = fromDate.Value;
                            tradingSimulationConfig.ToDate = toDate.Value;
                        }
                    }

                    var allQuotesPrefilterd = _stockQuoteRepository
                        .GetAll(x => !_projectSettings.BlackListPattern.IsMatch(x.Ticker)
                                     && x.DateParsed.InOpenRange(tradingSimulationConfig.FromDate.AddDays(-30), tradingSimulationConfig.ToDate))
                        .ToList();

                    var simulationResult = _tradingSimulator.Simulate(allQuotesPrefilterd, tradingSimulationConfig, _progressReporter);

                    _logger.LogInfo(simulationResult.ToString());
                    break;

                case "predict":
                    if (args.Length >= 2)
                    {
                        var date = TryParseDateTime(args[1]);
                        if (date.HasValue)
                        {
                            var prediction = _tradingSimulator.GetSignals(tradingSimulationConfig, date.Value);
                            _logger.LogInfo($"Stonks to buy: {string.Join(", ", prediction.Select(x => x.Ticker).OrderBy(x => x))}. Used prediction made with data from session {prediction.Select(x => x.DateParsed).First()} and config = {tradingSimulationConfig}");
                        }
                        else
                        {
                            _logger.LogError($"{args[1]} is not a valid date. Enter date in format YYYY-MM-DD");
                        }
                    }
                    else
                    {
                        _logger.LogError($"Enter date in format YYYY-MM-DD as second argument after empty space. Example: predict 2020-01-01");
                    }
                    break;

                default:
                    _logger.LogWarning(@$"{command} not recognized as valid command. {HelpMessage}");
                    break;
            }
        }

        private static DateTime? TryParseDateTime(string input)
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
- print: print selected stock quotes or all Tickers if no Ticker provided
- simulate: run trading simulation
- predict: get prediction for selected day
- cleanDir: cleans output directory
- unzip: unzip archive to output dir
- cleanLogs: remove all log files
- getDir: print working directory path
- openDir: start explorer with working directory path
- printUnzipped: print list of files in unzipped directory";
    }
}
