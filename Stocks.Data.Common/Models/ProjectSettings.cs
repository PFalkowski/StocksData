using Extensions.Standard;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using System;
using System.IO;
using System.Linq;
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

        public string ProjectName => _configuration[nameof(ProjectName)];

        public string OutputDirName => _configuration[nameof(OutputDirName)];

        public string ArchiveFileName => _configuration[nameof(ArchiveFileName)];

        public string UnzippedFilesDirectoryName => _configuration[nameof(UnzippedFilesDirectoryName)];

        public string QuotesFileExtension => _configuration[nameof(QuotesFileExtension)];

        public string LogFileName => _configuration[nameof(LogFileName)];

        public string QuotesDownloadUrl => _configuration[nameof(QuotesDownloadUrl)];

        public string ConnectionString => _configuration[nameof(ConnectionString)];

        public bool ExcludeBlacklisted => bool.Parse(_configuration[nameof(ExcludeBlacklisted)]);

        public string BlacklistPatternString => _configuration[nameof(BlacklistPatternString)];

        public bool ExcludePennyStocks => bool.Parse(_configuration[nameof(ExcludePennyStocks)]);

        public double PennyStockThreshold => double.Parse(_configuration[nameof(PennyStockThreshold)]);

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

        public override string ToString()
        {
            var settings = this.GetAllPublicPropertiesValues();

            return string.Join(", ", settings.Select(x => $"{x.Key} = {x.Value}"));
        }
    }
}
