using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Threading;
using CsvHelper;

namespace Stocks.Data.Csv
{
    public class CsvContext<TEntity> where TEntity : class
    {
        private ReaderWriterLockSlim LockSlim { get; } = new ReaderWriterLockSlim();
        public FileInfo File { get; set; }
        public CultureInfo Culture { get; set; }

        private List<TEntity> Entities { get;  set; } = new List<TEntity>();

        public List<TEntity> Set(List<TEntity> entities)
        {
            LockSlim.EnterWriteLock();
            try
            {
                //if (File.Exists)
                //{
                //    using (var csv = new CsvReader(File.OpenText(), false))
                //    {
                //        Entities = csv.GetRecords<TEntity>().ToList();
                //    }
                //}
                Entities.AddRange(entities);
                return Entities;
            }
            finally
            {
                LockSlim.ExitWriteLock();
            }
        }

        public CsvContext(FileInfo file)
        {
            File = file;
            LockSlim.EnterWriteLock();
            try
            {
                if (file.Exists)
                {
                    using (var csv = new CsvReader(file.OpenText(), false))
                    {
                        Entities = csv.GetRecords<TEntity>().ToList();
                    }
                }
                else
                {
                    Entities = new List<TEntity>();
                }
            }
            finally
            {
                LockSlim.ExitWriteLock();
            }
        }
        //public CsvContext(string filePath)
        //{
        //    File = new FileInfo(filePath);
        //    if (File.Exists)
        //    {
        //        using (var csv = new CsvReader(File.OpenText(), false))
        //        {
        //            Entities = csv.GetRecords<TEntity>().ToList();
        //        }
        //    }
        //    else
        //    {
        //        File.Create();
        //        Entities = new List<TEntity>();
        //    }
        //}

        public virtual void SaveChanges()
        {
            LockSlim.EnterWriteLock();
            try
            {
                using (var writer = new CsvWriter(File.CreateText(), false))
                {
                    writer.Configuration.SanitizeForInjection = true;
                    writer.Configuration.CultureInfo = Culture ?? CultureInfo.CurrentCulture;
                    writer.WriteRecords(Entities);
                    writer.Flush();
                }
            }
            finally
            {
                LockSlim.ExitWriteLock();
            }
        }
    }
}