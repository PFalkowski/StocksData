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

        public virtual int Count()
            => Entities.Count();

        public virtual int Count(Expression<Func<TEntity, bool>> predicate)
            => Entities.Count(predicate.Compile());

        public virtual TEntity GetById(params object[] id)
            => Entities.Find(id);

        public virtual TEntity Get(Expression<Func<TEntity, bool>> predicate)
            => Entities.FirstOrDefault(predicate.Compile());

        public virtual IEnumerable<TEntity> GetAll(Expression<Func<TEntity, bool>> predicate)
            => Entities.Where(predicate.Compile());

        public virtual IEnumerable<TEntity> GetAll()
            => Entities;

        public virtual void Add(TEntity entity)
            => Entities.Add(entity);

        public virtual void AddRange(IEnumerable<TEntity> entities)
            => Entities.AddRange(entities);

        public virtual void Remove(TEntity entity)
            => Entities.Remove(entity);

        public virtual void RemoveAll()
            => RemoveAll(x => true);

        public virtual void RemoveAll(Expression<Func<TEntity, bool>> predicate)
        {
            var toBeRemoved = GetAll(predicate).ToList();
            Entities.RemoveRange(toBeRemoved);
        }

        public virtual void RemoveRange(IEnumerable<TEntity> entities)
        {
            if (entities == null) throw new ArgumentNullException(nameof(entities));
            var list = entities.ToList();
            Entities.RemoveRange(list);
        }

        public virtual void AddOrUpdate(TEntity entity)
            => Entities.Update(entity);

        public virtual void Dispose()
        {
            Context?.Dispose();
        }
    }
}
