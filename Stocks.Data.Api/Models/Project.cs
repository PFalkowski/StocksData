using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace Stocks.Data.Api.Models
{
    public class Project
    {
        public string Name { get; set; } = "StockAnalysisProject1";
        public string OutputDirName { get; set; } = "Output";
        public string ArchiveFileName { get; set; } = "mstcgl.zip";
        public string UnzippedFilesDirectoryName { get; set; } = "mstcgl";
        public string QuotesFileExtension { get; set; } = "mst";
        public string LogFileName { get; set; } = "StockAnalysisProjectLog";
        public string QuotesDownloadUrl { get; set; } = "http://bossa.pl/pub/ciagle/mstock/mstcgl.zip";
        public string ConnectionString { get; set; } = $"server=(localdb)\\MSSQLLocalDB;Initial Catalog=StockQuotes;Integrated Security=True;";

        public DirectoryInfo WorkingDirectory => new DirectoryInfo(Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), Name, OutputDirName));
        public DirectoryInfo UnzippedFilesDirectory => new DirectoryInfo(Path.Combine(WorkingDirectory.FullName, UnzippedFilesDirectoryName));
        public FileInfo ArchiveFile => new FileInfo(Path.Combine(WorkingDirectory.FullName, ArchiveFileName));

        public void EnsureAllDirectoriesExist()
        {
            if (!WorkingDirectory.Exists)
            {
                WorkingDirectory.Create();
            }
            if (!UnzippedFilesDirectory.Exists)
            {
                UnzippedFilesDirectory.Create();
            }
        }
    }
}
