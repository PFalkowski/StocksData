using Microsoft.EntityFrameworkCore;
using Stocks.Data.Common.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text.RegularExpressions;

namespace Stocks.Data.UnitTests.Ef.Test.Config
{
    public class TestProjectSettings : IProjectSettings
    {
        public Dictionary<string, string> SettingsDictionary { get; }
        public string Name { get; set; }
        public string OutputDirName { get; set; }
        public string ArchiveFileName { get; set; }
        public string UnzippedFilesDirectoryName { get; set; }
        public string QuotesFileExtension { get; set; }
        public string LogFileName { get; set; }
        public string QuotesDownloadUrl { get; set; }
        public string ConnectionString { get; set; }
        public string BlacklistPatternString { get; set; }
        public Regex BlackListPattern { get; }
        public bool ExcludePennyStocks { get; set; }
        public double PennyStockThreshold { get; set; }
        public DirectoryInfo WorkingDirectory { get; }
        public DirectoryInfo UnzippedFilesDirectory { get; }
        public FileInfo ArchiveFile { get; }
        public DbContextOptions<DbContext> GetDbContextOptions => new DbContextOptionsBuilder<DbContext>()
            .UseInMemoryDatabase(Path.GetFileNameWithoutExtension(Path.GetRandomFileName()))
            .Options;
        public void EnsureAllDirectoriesExist()
        {
            throw new NotImplementedException();
        }

        public void ParseSettings(string[] args)
        {
            throw new NotImplementedException();
        }
    }
}
