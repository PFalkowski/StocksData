using System;
using StandardInterfaces;
using Stocks.Data.Model;

namespace Stocks.Data.Ef
{
    public interface ICompanyRepository : IRepository<Company>, IDisposable
    {
        Company GetById(string ticker);
    }
}