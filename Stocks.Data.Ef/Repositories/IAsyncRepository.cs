using System.Collections.Generic;
using System.Threading.Tasks;

namespace Stocks.Data.Ef.Repositories
{
    public interface IAsyncRepository<TEntity> where TEntity : class
    {
        Task AddAsync(TEntity entity);
        Task AddRangeAsync(IEnumerable<TEntity> entities);
        Task<int> SaveChangesAsync();
    }
}