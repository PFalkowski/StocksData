using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using StandardInterfaces;

namespace Stocks.Data.Ef.Repositories
{
    public class ReadOnlyRepository<TEntity> : IReadOnlyRepository<TEntity> where TEntity : class
    {
        protected DbContext Context { get; }

        protected DbSet<TEntity> Entities { get; }

        public ReadOnlyRepository(DbContext context)
        {
            Context = context;
            Entities = Context.Set<TEntity>();
        }

        public virtual int Count()
            => Entities.Count();

        public virtual async Task<int> CountAsync()
            => await Entities.CountAsync();
        
        public virtual int Count(Expression<Func<TEntity, bool>> predicate)
            => Entities.Count(predicate.Compile());

        public virtual async Task<int> CountAsync(Expression<Func<TEntity, bool>> predicate)
            => await Entities.CountAsync(predicate);
        
        public virtual TEntity GetById(params object[] id)
            => Entities.Find(id);

        public virtual async Task<TEntity> GetByIdAsync(params object[] id)
            => await Entities.FindAsync(id);
        
        public virtual TEntity Get(Expression<Func<TEntity, bool>> predicate)
            => Entities.FirstOrDefault(predicate.Compile());

        public virtual async Task<TEntity> GetAsync(Expression<Func<TEntity, bool>> predicate)
            => await Entities.FirstOrDefaultAsync(predicate);
        
        public virtual IEnumerable<TEntity> GetAll()
            => Entities;

        public virtual IEnumerable<TEntity> GetAll(Expression<Func<TEntity, bool>> predicate)
            => Entities.Where(predicate.Compile());

        public virtual void Dispose()
        {
            Context?.Dispose();
        }

        public virtual async Task DisposeAsync()
        {
            if (Context != null)
            {
                await Context.DisposeAsync();
            }
        }
    }
}
