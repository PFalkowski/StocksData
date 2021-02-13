using System.Collections.Generic;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using Stocks.Data.Model;

namespace Stocks.Data.Ef
{
    public class CompanyRepository : Repository<Company>, ICompanyRepository
    {
        public CompanyRepository(DbContext context) : base(context)
        {
        }

        public Company GetById(string ticker)
        {
            return base.Entities
                .Where(x => x.Ticker.Equals(ticker))
                .Include(x => x.Quotes)
                .SingleOrDefault();
        }

        public override IEnumerable<Company> GetAll()
        {
            return Entities
                .Include(x => x.Quotes);
        }
    }
}
