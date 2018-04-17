using FluentNHibernate.Cfg;
using FluentNHibernate.Cfg.Db;
using FluentNHibernate.Conventions.Helpers;
using NHibernate;
using NHibernate.Dialect;
using NHibernate.Tool.hbm2ddl;

namespace Stocks.Data.Nh
{
    public class StockNhContextModelUpdate : INhContext
    {
        public ISessionFactory SessionFactory { get; }
        public StockNhContextModelUpdate(string connectionString)
        {
            SessionFactory = Fluently.Configure()
                    .Database(MsSqlConfiguration.MsSql2012
                        .ConnectionString(connectionString).ShowSql())
                    .Mappings(m =>
                        m.FluentMappings
                            .Add<CompanyNhibernateMap>().Conventions.Add(DefaultCascade.All())
                            .Add<StockQuoteNhibernateMap>().Conventions.Add(DefaultCascade.All())
                            )
                            .ExposeConfiguration(cfg => SchemaMetadataUpdater.QuoteTableAndColumns(cfg, new MsSql2012Dialect()))
                            .ExposeConfiguration(cfg => new SchemaExport(cfg).Execute(true, true, false))
                            .BuildSessionFactory();

        }
    }
}
