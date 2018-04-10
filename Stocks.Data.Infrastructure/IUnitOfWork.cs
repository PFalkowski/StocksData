using System;

namespace Stocks.Data.Infrastructure
{
    public interface IUnitOfWork : IDisposable
    {
        void Complete();
    }
}
