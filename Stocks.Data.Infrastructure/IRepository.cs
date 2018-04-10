using System;
using System.Collections.Generic;

namespace Stocks.Data.Infrastructure
{
    public interface IRepository<TEntity> : IDisposable where TEntity : class
    {
        IEnumerable<TEntity> Entities { get; }
        //TEntity Get(Expression<Func<TEntity, bool>> predicate);
        //IEnumerable<TEntity> GetAll(Expression<Func<TEntity, bool>> predicate);
        //void RemoveAll(Expression<Func<TEntity, bool>> predicate);
        IList<TEntity> GetAll();
        void Add(TEntity entity);
        void AddOrUpdate(TEntity entity);
        void AddRange(IEnumerable<TEntity> entities);
        void Remove(TEntity entity);
        void RemoveRange(IEnumerable<TEntity> entities);
        int Count();
    }
}