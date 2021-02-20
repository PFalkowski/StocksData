using System.Collections.Generic;
using System.IO;
using System.Text.RegularExpressions;
using Microsoft.EntityFrameworkCore;

namespace Stocks.Data.Common.Models
{
    public interface IProjectSettings
    {
        string ProjectName { get; }
        string OutputDirName { get;  }
        string ArchiveFileName { get; }
        string UnzippedFilesDirectoryName { get; }
        string QuotesFileExtension { get;  }
        string QuotesDownloadUrl { get; }
        string ConnectionString { get;  }
        string BlacklistPatternString { get; }
        string LogFileName { get;  }
        bool ExcludeBlacklisted { get;  }
        Regex BlackListPattern { get; }
        bool ExcludePennyStocks { get;  }
        double PennyStockThreshold { get;  }
        DirectoryInfo WorkingDirectory { get; }
        DirectoryInfo UnzippedFilesDirectory { get; }
        FileInfo ArchiveFile { get; }
        DbContextOptions<DbContext> GetDbContextOptions { get; }
        void EnsureAllDirectoriesExist();
        void CleanOutputDirectory();
    }
}