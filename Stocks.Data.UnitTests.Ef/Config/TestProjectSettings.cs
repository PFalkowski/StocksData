using Microsoft.EntityFrameworkCore;
using Stocks.Data.Common.Models;
using System;
using System.IO;
using System.Text.RegularExpressions;

namespace Stocks.Data.UnitTests.Ef.Test.Config
{
    public class TestProjectSettings : IProjectSettings
    {
        public string ProjectName { get; set; }
        public string OutputDirName { get; set; }
        public string ArchiveFileName { get; set; }
        public string UnzippedFilesDirectoryName { get; set; }
        public string QuotesFileExtension { get; set; }
        public string LogDirectoryName { get; }
        public string LogFileNameBase { get; set; }
        public bool ExcludeBlacklisted { get; }
        public string QuotesDownloadUrl { get; set; }
        public string QuotesUpdateUrlBase { get; }
        public string ConnectionString { get; set; }
        public string BlacklistPatternString { get; set; }
        public Regex BlackListPattern { get; }
        public bool ExcludePennyStocks { get; set; }
        public double PennyStockThreshold { get; set; }
        public DirectoryInfo WorkingDirectory { get; }
        public DirectoryInfo UnzippedFilesDirectory { get; }
        public DirectoryInfo LogDirectory { get; }
        public FileInfo ArchiveFile { get; }
        public DbContextOptions<DbContext> GetDbContextOptions => new DbContextOptionsBuilder<DbContext>()
            .UseInMemoryDatabase(Path.GetFileNameWithoutExtension(Path.GetRandomFileName()))
            .Options;
        public void EnsureAllDirectoriesExist()
        {
            throw new NotImplementedException();
        }

        public void CleanOutputDirectory()
        {
            throw new NotImplementedException();
        }

        public void CleanLogs()
        {
            throw new NotImplementedException();
        }
    }
}
