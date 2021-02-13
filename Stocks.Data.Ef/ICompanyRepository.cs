using StandardInterfaces;
using Stocks.Data.Model;

namespace Stocks.Data.Ef
{
    public interface ICompanyRepository : IRepository<Company>
    {
        Company GetById(string ticker);
    }
}