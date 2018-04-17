using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using NHibernate;
using Stocks.Data.Infrastructure;

namespace Stocks.Data.Nh
{
    public class NhRepository<T> : IRepository<T> where T : class
    {
        private IEnumerable<T> Entities => Session.Query<T>();

        public ISession Session { get; }

        public int Count() => Entities.Count();

        public int Count(Expression<Func<T, bool>> predicate)
        {
            return Session.Query<T>().Count(predicate);
        }

        public NhRepository(ISession session)
        {
            Session = session;
        }
        public void Add(T entity)
        {
            Session.Save(entity);
            Session.Flush();
        }

        public void AddOrUpdate(T entity)
        {
            Session.SaveOrUpdate(entity);
            Session.Flush();
        }

        public void AddRange(IEnumerable<T> entities)
        {
            foreach (var entity in entities)
            {
                Session.Save(entity);
                Session.Flush();
            }
        }

        public T Get(Expression<Func<T, bool>> predicate)
        {
            return Session.Query<T>().FirstOrDefault(predicate);
        }

        public IEnumerable<T> GetAll(Expression<Func<T, bool>> predicate)
        {
            return Session.Query<T>().Where(predicate);
        }

        public List<T> GetAll()
        {
            return Entities.ToList();
        }

        public void Remove(T entity)
        {
            Session.Delete(entity);
            Session.Flush();
        }

        public void RemoveRange(IEnumerable<T> entities)
        {
            var list = entities.ToList();
            foreach (var entity in list)
            {
                Remove(entity);
                Session.Flush();
            }
        }

        public void RemoveAll(Expression<Func<T, bool>> predicate)
        {
            var elementsToRemove = Session.Query<T>().Where(predicate);
            RemoveRange(elementsToRemove);
        }

        public void RemoveAll()
        {
            RemoveRange(Entities);
        }

        public void Dispose()
        {
            Session?.Dispose();
        }
    }
}
