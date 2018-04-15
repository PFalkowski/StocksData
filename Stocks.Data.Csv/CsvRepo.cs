using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using Stocks.Data.Infrastructure;

namespace Stocks.Data.Csv
{
    public class CsvRepo<TEntity> : IRepository<TEntity> where TEntity : class
    {
        private CsvContext<TEntity> Context { get; set; }
        private HashSet<TEntity> Entities { get; set; } = new HashSet<TEntity>();

        public int Count() => Entities.Count;

        public CsvRepo(CsvContext<TEntity> context)
        {
            Context = context;
            Entities = Context.Set(Entities);
        }

        public void Add(TEntity entity)
        {
            var result = Entities.Add(entity);
            if (!result) throw new ArgumentException($"{entity} already in context");
        }

        public void AddRange(IEnumerable<TEntity> entities)
        {
            foreach (var entity in entities)
            {
                Add(entity);
            }
        }

        public TEntity Get(TEntity id)
        {
            return Entities.First(x => x.Equals(id));
        }


        public TEntity Get(Expression<Func<TEntity, bool>> predicate)
        {
            return Entities.First(predicate.Compile());
        }

        public int Count(Expression<Func<TEntity, bool>> predicate)
        {
            return Entities.Count(predicate.Compile());
        }

        public IEnumerable<TEntity> GetAll(Expression<Func<TEntity, bool>> predicate)
        {
            return Entities.Where(predicate.Compile());
        }

        public List<TEntity> GetAll()
        {
            return Entities.ToList();
        }

        public void Remove(TEntity entity)
        {
            Entities.Remove(entity);
        }

        public void RemoveRange(IEnumerable<TEntity> entities)
        {
            var list = entities.ToList(); // prevent 'Collection was modified' error, when enumerating and deleting from same collection
            foreach (var entity in list)
            {
                Remove(entity);
            }
        }

        public void RemoveAll(Expression<Func<TEntity, bool>> predicate)
        {
            var toBeRemoved = GetAll(predicate).ToList();
            RemoveRange(toBeRemoved);
        }

        public void RemoveAll()
        {
            RemoveAll(x => true);
        }

        public void AddOrUpdate(TEntity entity)
        {
            var found = Get(entity);
            if (found != null)
            {
                Entities.Remove(found);
            }
            Add(entity);
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
