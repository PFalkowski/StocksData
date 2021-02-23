using System.Collections.Generic;
using System.IO;
using System.Text.RegularExpressions;
using Microsoft.EntityFrameworkCore;

namespace Stocks.Data.Common.Models
{
    public interface IProjectSettings
    {
        string ProjectName { get; }
        string OutputDirName { get; }
        string ArchiveFileName { get; }
        string UnzippedFilesDirectoryName { get; }
        string QuotesFileExtension { get; }
        string QuotesDownloadUrl { get; }
        string QuotesUpdateUrlBase { get; }
        string ConnectionString { get; }
        string BlacklistPatternString { get; }
        string LogDirectoryName { get; }
        string LogFileNameBase { get; }
        bool ExcludeBlacklisted { get; }
        bool UserInteractive { get; }
        string Run { get; }
        bool ShouldRunTasks { get; }
        Regex BlackListPattern { get; }
        DirectoryInfo WorkingDirectory { get; }
        DirectoryInfo UnzippedFilesDirectory { get; }
        DirectoryInfo LogDirectory { get; }
        FileInfo ArchiveFile { get; }
        DbContextOptions<DbContext> GetDbContextOptions { get; }
        void EnsureAllDirectoriesExist();
        void CleanOutputDirectory();
        void CleanLogs();
        IEnumerable<string> GetFilesListInDirectory(DirectoryInfo directory);
    }
}