using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore;
using StandardInterfaces;

namespace Stocks.Data.Ef
{
    public class Repository<TEntity> : ReadOnlyRepository<TEntity>, IRepository<TEntity> where TEntity : class
    {
        public Repository(DbContext context) : base(context) { }

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
    }
}
