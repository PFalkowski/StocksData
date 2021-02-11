﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace Stocks.Data.Api.Models
{
    public class ProjectSettings
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
                }
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

        public DirectoryInfo WorkingDirectory => new DirectoryInfo(Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), Name, OutputDirName));
        public DirectoryInfo UnzippedFilesDirectory => new DirectoryInfo(Path.Combine(WorkingDirectory.FullName, UnzippedFilesDirectoryName));
        public FileInfo ArchiveFile => new FileInfo(Path.Combine(WorkingDirectory.FullName, ArchiveFileName));

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
    }
}