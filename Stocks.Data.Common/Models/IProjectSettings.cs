﻿using System.Collections.Generic;
using System.IO;
using Microsoft.EntityFrameworkCore;

namespace Stocks.Data.Common.Models
{
    public interface IProjectSettings
    {
        Dictionary<string, string> SettingsDictionary { get; }
        string Name { get; set; }
        string OutputDirName { get; set; }
        string ArchiveFileName { get; set; }
        string UnzippedFilesDirectoryName { get; set; }
        string QuotesFileExtension { get; set; }
        string LogFileName { get; set; }
        string QuotesDownloadUrl { get; set; }
        string ConnectionString { get; set; }
        DirectoryInfo WorkingDirectory { get; }
        DirectoryInfo UnzippedFilesDirectory { get; }
        FileInfo ArchiveFile { get; }
        DbContextOptions<DbContext> GetDbContextOptions { get; }
        void EnsureAllDirectoriesExist();
        void ParseSettings(string[] args);
    }
}