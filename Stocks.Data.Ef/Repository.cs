using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore;
using StandardInterfaces;

namespace Stocks.Data.Ef
{
    public class Repository<TEntity> : IRepository<TEntity> where TEntity : class
    {
        private DbContext Context { get; }

        protected DbSet<TEntity> Entities { get; }

        public Repository(DbContext context)
        {
            Context = context;
            Entities = Context.Set<TEntity>();
        }

        public int Count()
            => Entities.Count();

        public int Count(Expression<Func<TEntity, bool>> predicate)
            => Entities.Count(predicate.Compile());

        public TEntity GetById(params object[] id)
            => Entities.Find(id);

        public TEntity Get(Expression<Func<TEntity, bool>> predicate)
            => Entities.FirstOrDefault(predicate.Compile());

        public IEnumerable<TEntity> GetAll(Expression<Func<TEntity, bool>> predicate)
            => Entities.Where(predicate.Compile());

        public IEnumerable<TEntity> GetAll()
            => Entities;

        public void Add(TEntity entity)
            => Entities.Add(entity);

        public void AddRange(IEnumerable<TEntity> entities)
            => Entities.AddRange(entities);

        public void Remove(TEntity entity)
            => Entities.Remove(entity);

        public void RemoveAll()
            => RemoveAll(x => true);

        public void RemoveAll(Expression<Func<TEntity, bool>> predicate)
        {
            var toBeRemoved = GetAll(predicate).ToList();
            Entities.RemoveRange(toBeRemoved);
        }

        public void RemoveRange(IEnumerable<TEntity> entities)
        {
            if (entities == null) throw new ArgumentNullException(nameof(entities));
            var list = entities.ToList();
            Entities.RemoveRange(list);
        }

        public void AddOrUpdate(TEntity entity)
            => Entities.Update(entity);

        public void Dispose()
        {
            Context?.Dispose();
        }
    }
}
