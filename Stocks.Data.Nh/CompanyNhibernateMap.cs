using FluentNHibernate.Mapping;
using Stocks.Data.Model;
using StocksData.Mappings;

namespace Stocks.Data.Nh
{

    public class CompanyNhibernateMap : ClassMap<Company>
    {
        public CompanyNhibernateMap()
        {
            Table(Constants.CompaniesName);
            Id(x => x.Ticker);
            HasMany(x => x.Quotes)
            .KeyColumn(Constants.TickerName)
            .Cascade.All()
            .Inverse();
        }
    }
}
