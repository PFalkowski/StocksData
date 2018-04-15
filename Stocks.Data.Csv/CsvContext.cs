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
        protected ReaderWriterLockSlim LockSlim { get; } = new ReaderWriterLockSlim();
        public FileInfo File { get; set; }
        public CultureInfo Culture { get; set; }

        private List<TEntity> Entities { get; set; } = new List<TEntity>();

        public List<TEntity> Set(List<TEntity> entities)
        {
            LockSlim.EnterReadLock();
            try
            {
                if (File.Exists)
                {
                    using (var csv = new CsvReader(File.OpenText(), false))
                    {
                        Entities = csv.GetRecords<TEntity>().ToList();
                    }
                }
                Entities.AddRange(entities);
                return Entities;
            }
            finally
            {
                LockSlim.ExitReadLock();
            }
        }

        public CsvContext(FileInfo file)
        {
            File = file;
        }

        public CsvContext(string filePath) : this(new FileInfo(filePath)) { }

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