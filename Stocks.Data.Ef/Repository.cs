using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using StandardInterfaces;

namespace Stocks.Data.Ef
{
    public class Repository<TEntity> : ReadOnlyRepository<TEntity>, IRepository<TEntity>, IAsyncRepository<TEntity> where TEntity : class
    {
        public Repository(DbContext context) : base(context) { }
        
        public virtual void Add(TEntity entity)
            => Entities.Add(entity);

        public virtual async Task AddAsync(TEntity entity)
            => await Entities.AddAsync(entity);
        
        public virtual void AddRange(IEnumerable<TEntity> entities)
            => Entities.AddRange(entities);

        public virtual async Task AddRangeAsync(IEnumerable<TEntity> entities)
            => await Entities.AddRangeAsync(entities);

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
        
        public virtual int SaveChanges()
        {
            return Context.SaveChanges();
        }

        public virtual async Task<int> SaveChangesAsync()
        {
            return await Context.SaveChangesAsync();
        }

        /// <summary>
        /// Rejects the changes. https://stackoverflow.com/a/22098063
        /// </summary>
        public void RejectChanges()
        {
            foreach (var entry in Context.ChangeTracker.Entries())
            {
                switch (entry.State)
                {
                    case EntityState.Modified:
                    case EntityState.Deleted:
                        entry.State = EntityState.Modified; //Revert changes made to deleted entity.
                        entry.State = EntityState.Unchanged;
                        break;
                    case EntityState.Added:
                        entry.State = EntityState.Detached;
                        break;
                }
            }
        }
    }
}
