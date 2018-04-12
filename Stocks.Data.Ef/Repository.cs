using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore;
using Stocks.Data.Infrastructure;

namespace Stocks.Data.Ef
{
    public class Repository<TEntity> : IRepository<TEntity> where TEntity : class
    {
        public DbContext Context { get; }

        public DbSet<TEntity> EntitiesDbSet { get; }

        public IEnumerable<TEntity> Entities => EntitiesDbSet;

        public int Count() => EntitiesDbSet.Count();

        public Repository(DbContext context)
        {
            Context = context;
            EntitiesDbSet = Context.Set<TEntity>();
        }

        public TEntity Get(object id)
        {
            return EntitiesDbSet.Find(id);
        }

        public TEntity Get(Expression<Func<TEntity, bool>> predicate)
        {
            return EntitiesDbSet.FirstOrDefault(predicate);
        }

        public IEnumerable<TEntity> GetAll(Expression<Func<TEntity, bool>> predicate)
        {
            return EntitiesDbSet.Where(predicate);
        }

        public IList<TEntity> GetAll()
        {
            return EntitiesDbSet.ToList();
        }

        public void Add(TEntity entity)
        {
            if (entity == null) throw new ArgumentNullException(nameof(entity));
            EntitiesDbSet.Add(entity);
        }

        public void AddRange(IEnumerable<TEntity> entities)
        {
            if (entities == null) throw new ArgumentNullException(nameof(entities));
            EntitiesDbSet.AddRange(entities);
        }

        public void Remove(TEntity entity)
        {
            EntitiesDbSet.Remove(entity);
        }

        public void RemoveAll(Expression<Func<TEntity, bool>> predicate)
        {
            var toBeRemoved = GetAll(predicate).ToList();
            EntitiesDbSet.RemoveRange(toBeRemoved);
        }

        public void RemoveRange(IEnumerable<TEntity> entities)
        {
            var list = entities.ToList();
            EntitiesDbSet.RemoveRange(list);
        }

        public void AddOrUpdate(TEntity entity)
        {
            //var found = EntitiesDbSet.Find(entity);
            //if (found == null)
            //{
            //    EntitiesDbSet.Add(entity);
            //}
            //else
            //{
                EntitiesDbSet.Update(entity);
            //}
        }

        public void Dispose()
        {
            Context?.Dispose();
        }
    }
}
