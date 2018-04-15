using System;
using System.Collections.Generic;
using Stocks.Data.Infrastructure;

namespace Stocks.Data.Csv
{
    public class CsvRepo<TEntity> : IRepository<TEntity> where TEntity : class
    {
        private CsvContext<TEntity> Context { get; set; }
        private List<TEntity> Entities { get; set; } = new List<TEntity>();

        public int Count() => Entities.Count;

        public CsvRepo(CsvContext<TEntity> context)
        {
            Context = context;
            Entities = Context.Set(Entities);
        }

        public void Add(TEntity entity)
        {
            Entities.Add(entity);
        }

        public void AddRange(IEnumerable<TEntity> entities)
        {
            Entities.AddRange(entities);
        }

        public TEntity Get(object id)
        {
            return Entities.Find(x => x.Equals(id));
        }

        public List<TEntity> GetAll(Predicate<TEntity> match)
        {
            return Entities.FindAll(match);
        }

        public List<TEntity> GetAll()
        {
            return Entities;
        }

        public void Remove(TEntity entity)
        {
            Entities.Remove(entity);
        }

        public void RemoveRange(IEnumerable<TEntity> entities)
        {
            foreach (var entity in entities)
            {
                Remove(entity);
            }
        }

        public void RemoveAll()
        {
            Entities.RemoveAll(x => true);
        }

        public void AddOrUpdate(TEntity entity)
        {
            var found = Get(entity);
            if (found != null)
            {
                found = entity;
            }
            else
            {
                Add(entity);
            }
        }

        public void AddRangeBulk(IEnumerable<TEntity> entities)
        {
            AddRange(entities);
        }

        public void AddBulk(TEntity entity)
        {
            Add(entity);
        }

        public void Dispose()
        {
        }
    }
}
