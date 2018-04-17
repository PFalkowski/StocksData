using NHibernate;

namespace Stocks.Data.Nh
{
    public interface INhContext
    {
        ISessionFactory SessionFactory { get; }
    }
}
