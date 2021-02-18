using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using Stocks.Data.Infrastructure;

namespace Stocks.Data.Common.Models
{
    public class ProjectSettings : IProjectSettings
    {
        public ProjectSettings()
        {
            SettingsDictionary = new Dictionary<string, string>
            {
                {nameof(Name), "StockAnalysisProject1"},
                {nameof(OutputDirName), "Output"},
                {nameof(ArchiveFileName), "mstcgl.zip"},
                {nameof(UnzippedFilesDirectoryName), "mstcgl"},
                {nameof(QuotesFileExtension), "mst"},
                {nameof(LogFileName), "StockAnalysisProjectLog"},
                {nameof(QuotesDownloadUrl), "http://bossa.pl/pub/ciagle/mstock/mstcgl.zip"},
                {
                    nameof(ConnectionString),
                    "server=(localdb)\\MSSQLLocalDB;Initial Catalog=StockQuotes;Integrated Security=True;"
                },
                {nameof(BlacklistPatternString), Constants.BlacklistPatternString},
                {nameof(PennyStockThreshold), Constants.PennyStockThreshold.ToString(CultureInfo.InvariantCulture)},
            };
        }
        public Dictionary<string, string> SettingsDictionary { get; private set; }

        public string Name
        {
            get => SettingsDictionary[nameof(Name)];
            set => SettingsDictionary[nameof(Name)] = value;
        }
        public string OutputDirName
        {
            get => SettingsDictionary[nameof(OutputDirName)];
            set => SettingsDictionary[nameof(OutputDirName)] = value;
        }
        public string ArchiveFileName
        {
            get => SettingsDictionary[nameof(ArchiveFileName)];
            set => SettingsDictionary[nameof(ArchiveFileName)] = value;
        }
        public string UnzippedFilesDirectoryName
        {
            get => SettingsDictionary[nameof(UnzippedFilesDirectoryName)];
            set => SettingsDictionary[nameof(UnzippedFilesDirectoryName)] = value;
        }
        public string QuotesFileExtension
        {
            get => SettingsDictionary[nameof(QuotesFileExtension)];
            set => SettingsDictionary[nameof(QuotesFileExtension)] = value;
        }
        public string LogFileName
        {
            get => SettingsDictionary[nameof(LogFileName)];
            set => SettingsDictionary[nameof(LogFileName)] = value;
        }
        public string QuotesDownloadUrl
        {
            get => SettingsDictionary[nameof(QuotesDownloadUrl)];
            set => SettingsDictionary[nameof(QuotesDownloadUrl)] = value;
        }
        public string ConnectionString
        {
            get => SettingsDictionary[nameof(ConnectionString)];
            set => SettingsDictionary[nameof(ConnectionString)] = value;
        }
        public string BlacklistPatternString
        {
            get => SettingsDictionary[nameof(BlacklistPatternString)];
            set => SettingsDictionary[nameof(BlacklistPatternString)] = value;
        }
        public bool ExcludePennyStocks
        {
            get => bool.Parse(SettingsDictionary[nameof(ExcludePennyStocks)]);
            set => SettingsDictionary[nameof(ExcludePennyStocks)] = value.ToString();
        }
        public double PennyStockThreshold
        {
            get => double.Parse(SettingsDictionary[nameof(PennyStockThreshold)]);
            set => SettingsDictionary[nameof(PennyStockThreshold)] = value.ToString(CultureInfo.InvariantCulture);
        }

        public Regex BlackListPattern => new Regex(BlacklistPatternString);
        public DirectoryInfo WorkingDirectory => new DirectoryInfo(Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), Name, OutputDirName));
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
            //if (!UnzippedFilesDirectory.Exists)
            //{
            //    UnzippedFilesDirectory.Create();
            //}
        }

        public void ParseSettings(string[] args)
        {
            foreach (var arg in args)
            {
                var split = arg.TrimStart('-').Split("-");
                var firstPart = split.FirstOrDefault();
                var secondPart = split.LastOrDefault();
                if (firstPart != null && secondPart != null &&
                    SettingsDictionary.ContainsKey(firstPart))
                {
                    SettingsDictionary[firstPart] = secondPart;
                }
            }
        }
    }
}
