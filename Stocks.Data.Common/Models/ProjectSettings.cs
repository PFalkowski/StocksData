using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Reflection;
using System.Text.RegularExpressions;

namespace Stocks.Data.Common.Models
{
    public class ProjectSettings : IProjectSettings
    {
        public ProjectSettings(IConfiguration configuration)
        {
            var settingsKeys = new List<string>();
            foreach (var propertyInfo in typeof(ProjectSettings).GetProperties(BindingFlags.Public | BindingFlags.Instance))
            {
                if (propertyInfo.CanWrite)
                {
                    settingsKeys.Add(propertyInfo.Name);
                }
            }
            foreach (var settingKey in settingsKeys)
            {
                _settingsDictionary[settingKey] = configuration[settingKey];
            }
        }

        private readonly Dictionary<string, string> _settingsDictionary = new Dictionary<string, string>();

        public string ProjectName
        {
            get => _settingsDictionary[nameof(ProjectName)];
            set => _settingsDictionary[nameof(ProjectName)] = value;
        }
        public string OutputDirName
        {
            get => _settingsDictionary[nameof(OutputDirName)];
            set => _settingsDictionary[nameof(OutputDirName)] = value;
        }
        public string ArchiveFileName
        {
            get => _settingsDictionary[nameof(ArchiveFileName)];
            set => _settingsDictionary[nameof(ArchiveFileName)] = value;
        }
        public string UnzippedFilesDirectoryName
        {
            get => _settingsDictionary[nameof(UnzippedFilesDirectoryName)];
            set => _settingsDictionary[nameof(UnzippedFilesDirectoryName)] = value;
        }
        public string QuotesFileExtension
        {
            get => _settingsDictionary[nameof(QuotesFileExtension)];
            set => _settingsDictionary[nameof(QuotesFileExtension)] = value;
        }
        public string LogFileName
        {
            get => _settingsDictionary[nameof(LogFileName)];
            set => _settingsDictionary[nameof(LogFileName)] = value;
        }
        public string QuotesDownloadUrl
        {
            get => _settingsDictionary[nameof(QuotesDownloadUrl)];
            set => _settingsDictionary[nameof(QuotesDownloadUrl)] = value;
        }
        public string ConnectionString
        {
            get => _settingsDictionary[nameof(ConnectionString)];
            set => _settingsDictionary[nameof(ConnectionString)] = value;
        }
        public bool ExcludeBlacklisted
        {
            get => bool.Parse(_settingsDictionary[nameof(ExcludeBlacklisted)]);
            set => _settingsDictionary[nameof(ExcludeBlacklisted)] = value.ToString();
        }
        public string BlacklistPatternString
        {
            get => _settingsDictionary[nameof(BlacklistPatternString)];
            set => _settingsDictionary[nameof(BlacklistPatternString)] = value;
        }
        public bool ExcludePennyStocks
        {
            get => bool.Parse(_settingsDictionary[nameof(ExcludePennyStocks)]);
            set => _settingsDictionary[nameof(ExcludePennyStocks)] = value.ToString();
        }
        public double PennyStockThreshold
        {
            get => double.Parse(_settingsDictionary[nameof(PennyStockThreshold)]);
            set => _settingsDictionary[nameof(PennyStockThreshold)] = value.ToString(CultureInfo.InvariantCulture);
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
