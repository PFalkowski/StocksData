using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Threading;
using CsvHelper;
using CsvHelper.Configuration;

namespace Stocks.Data.Csv
{
    public class CsvContext<TEntity> where TEntity : class
    {
        protected readonly object SyncRoot = new object();
        public FileInfo File { get; set; }
        public CultureInfo Culture { get; set; }
        public ClassMap<TEntity> CustomMapping { get; set; }

        public HashSet<TEntity> Entities { get; protected set; } = new HashSet<TEntity>();

        public HashSet<TEntity> Set(HashSet<TEntity> entities)
        {
            lock (SyncRoot)
            {
                if (File.Exists)
                {
                    using (var reader = new CsvReader(File.OpenText(), false))
                    {
                        reader.Configuration.CultureInfo = Culture ?? CultureInfo.CurrentCulture;
                        if (CustomMapping != null)
                        {
                            reader.Configuration.RegisterClassMap(CustomMapping);
                        }
                        Entities = new HashSet<TEntity>(reader.GetRecords<TEntity>());
                    }
                }
                foreach (var entity in entities)
                {
                    var result = Entities.Add(entity);
                    if (!result) throw new ArgumentException($"{entity} already in context");
                }
                return Entities;

            }
        }

        public CsvContext(FileInfo file)
        {
            File = file;
        }

        public CsvContext(string filePath) : this(new FileInfo(filePath)) { }

        public virtual void SaveChanges()
        {
            lock (SyncRoot)
            {
                using (var writer = new CsvWriter(File.CreateText(), false))
                {
                    writer.Configuration.SanitizeForInjection = true;
                    writer.Configuration.CultureInfo = Culture ?? CultureInfo.CurrentCulture;
                    if (CustomMapping != null)
                    {
                        writer.Configuration.RegisterClassMap(CustomMapping);
                    }

                    writer.WriteRecords(Entities);
                    writer.Flush();
                }
            }
        }
    }
}
