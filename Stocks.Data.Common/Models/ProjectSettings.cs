using Extensions.Standard;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
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
        
        public string LogDirectoryName => _configuration[nameof(LogDirectoryName)];

        public string LogFileNameBase => _configuration[nameof(LogFileNameBase)];
        
        public string QuotesDownloadUrl => _configuration[nameof(QuotesDownloadUrl)];

        public string QuotesUpdateUrlBase => _configuration[nameof(QuotesUpdateUrlBase)];

        public string ConnectionString => _configuration[nameof(ConnectionString)];

        public bool ExcludeBlacklisted => bool.Parse(_configuration[nameof(ExcludeBlacklisted)]);

        public string BlacklistPatternString => _configuration[nameof(BlacklistPatternString)];

        public bool UserInteractive => bool.Parse(_configuration[nameof(UserInteractive)]);

        public string Run => _configuration[nameof(Run)];

        public bool ShouldRunTasks => !string.IsNullOrEmpty(Run);


        public Regex BlackListPattern => new Regex(BlacklistPatternString);//, RegexOptions.Compiled ?
        public DirectoryInfo WorkingDirectory => new DirectoryInfo(Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), ProjectName, OutputDirName));
        public DirectoryInfo UnzippedFilesDirectory => new DirectoryInfo(Path.Combine(WorkingDirectory.FullName, UnzippedFilesDirectoryName));
        public FileInfo ArchiveFile => new FileInfo(Path.Combine(WorkingDirectory.FullName, ArchiveFileName));
        public DbContextOptions<DbContext> GetDbContextOptions => new DbContextOptionsBuilder<DbContext>()
            .UseSqlServer(ConnectionString)
            .Options;
        public DirectoryInfo LogDirectory => new DirectoryInfo(Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), ProjectName, LogDirectoryName));


        public void EnsureAllDirectoriesExist()
        {
            if (!WorkingDirectory.Exists)
            {
                WorkingDirectory.Create();
            }
            if (!LogDirectory.Exists)
            {
                LogDirectory.Create();
            }
        }
        
        public void CleanOutputDirectory()
        {
            foreach (var file in UnzippedFilesDirectory.GetFiles())
            {
                file.Delete();
            }
        }

        public void CleanLogs()
        {
            foreach (var file in LogDirectory.GetFiles())
            {
                file.Delete();
            }
        }
        public IEnumerable<string> GetFilesListInDirectory(DirectoryInfo directory)
        {
            return directory.GetFiles().Select(x => x.Name);
        }


        public override string ToString()
        {
            var settings = this.GetAllPublicPropertiesValues();

            return string.Join(", ", settings.Select(x => $"{x.Key} = {x.Value}"));
        }
    }
}
