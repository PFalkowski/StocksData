using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using System;
using System.Globalization;
using System.IO;
using System.Text.RegularExpressions;

namespace Stocks.Data.Common.Models
{
    public class ProjectSettings : IProjectSettings
    {
        private readonly IConfiguration _configuration;

        public ProjectSettings(IConfiguration configuration)
        {
            _configuration = configuration;
        }
        
        public string ProjectName
        {
            get => _configuration[nameof(ProjectName)];
            set => _configuration[nameof(ProjectName)] = value;
        }
        public string OutputDirName
        {
            get => _configuration[nameof(OutputDirName)];
            set => _configuration[nameof(OutputDirName)] = value;
        }
        public string ArchiveFileName
        {
            get => _configuration[nameof(ArchiveFileName)];
            set => _configuration[nameof(ArchiveFileName)] = value;
        }
        public string UnzippedFilesDirectoryName
        {
            get => _configuration[nameof(UnzippedFilesDirectoryName)];
            set => _configuration[nameof(UnzippedFilesDirectoryName)] = value;
        }
        public string QuotesFileExtension
        {
            get => _configuration[nameof(QuotesFileExtension)];
            set => _configuration[nameof(QuotesFileExtension)] = value;
        }
        public string LogFileName
        {
            get => _configuration[nameof(LogFileName)];
            set => _configuration[nameof(LogFileName)] = value;
        }
        public string QuotesDownloadUrl
        {
            get => _configuration[nameof(QuotesDownloadUrl)];
            set => _configuration[nameof(QuotesDownloadUrl)] = value;
        }
        public string ConnectionString
        {
            get => _configuration[nameof(ConnectionString)];
            set => _configuration[nameof(ConnectionString)] = value;
        }
        public bool ExcludeBlacklisted
        {
            get => bool.Parse(_configuration[nameof(ExcludeBlacklisted)]);
            set => _configuration[nameof(ExcludeBlacklisted)] = value.ToString();
        }
        public string BlacklistPatternString
        {
            get => _configuration[nameof(BlacklistPatternString)];
            set => _configuration[nameof(BlacklistPatternString)] = value;
        }
        public bool ExcludePennyStocks
        {
            get => bool.Parse(_configuration[nameof(ExcludePennyStocks)]);
            set => _configuration[nameof(ExcludePennyStocks)] = value.ToString();
        }
        public double PennyStockThreshold
        {
            get => double.Parse(_configuration[nameof(PennyStockThreshold)]);
            set => _configuration[nameof(PennyStockThreshold)] = value.ToString(CultureInfo.InvariantCulture);
        }

        public Regex BlackListPattern => new Regex(BlacklistPatternString);//, RegexOptions.Compiled ?
        public DirectoryInfo WorkingDirectory => new DirectoryInfo(Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), ProjectName, OutputDirName));
        public DirectoryInfo UnzippedFilesDirectory => new DirectoryInfo(Path.Combine(WorkingDirectory.FullName, UnzippedFilesDirectoryName));
        public FileInfo ArchiveFile => new FileInfo(Path.Combine(WorkingDirectory.FullName, ArchiveFileName));
        public DbContextOptions<DbContext> GetDbContextOptions => new DbContextOptionsBuilder<DbContext>()
            .UseSqlServer(ConnectionString)
            .Options;

        public void EnsureAllDirectoriesExist()
        {
            if (!WorkingDirectory.Exists)
            {
                WorkingDirectory.Create();
            }
        }

        public void CleanOutputDirectory()
        {
            foreach (var file in UnzippedFilesDirectory.GetFiles())
            {
                file.Delete();
            }
        }
    }
}
