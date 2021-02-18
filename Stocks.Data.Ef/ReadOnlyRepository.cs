using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using Microsoft.EntityFrameworkCore;
using StandardInterfaces;

namespace Stocks.Data.Ef
{
    public class ReadOnlyRepository<TEntity> : IReadOnlyRepository<TEntity> where TEntity : class
    {
        private DbContext Context { get; }

        protected DbSet<TEntity> Entities { get; }

        public ReadOnlyRepository(DbContext context)
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

        public virtual void Dispose()
        {
            Context?.Dispose();
        }
    }
}
