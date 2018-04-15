using System;
using System.Collections.Generic;
using System.Linq.Expressions;

namespace Stocks.Data.Infrastructure
{
    public interface IRepository<TEntity> : IDisposable where TEntity : class
    {
        TEntity Get(Expression<Func<TEntity, bool>> predicate);
        IEnumerable<TEntity> GetAll(Expression<Func<TEntity, bool>> predicate);
        void RemoveAll(Expression<Func<TEntity, bool>> predicate);
        int Count(Expression<Func<TEntity, bool>> predicate);
        List<TEntity> GetAll();
        void Add(TEntity entity);
        void AddOrUpdate(TEntity entity);
        void AddRange(IEnumerable<TEntity> entities);
        void Remove(TEntity entity);
        void RemoveRange(IEnumerable<TEntity> entities);
        void RemoveAll();
        int Count();
    }
}