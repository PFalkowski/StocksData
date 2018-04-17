using Stocks.Data.Model;

namespace Stocks.Data.Nh
{
    public class StockNhUnitOfWork : NhUnitOfWork
    {
        public NhRepository<Company> Stocks { get; }
        public StockNhUnitOfWork(INhContext context) : base(context)
        {
            Stocks = new NhRepository<Company>(context.SessionFactory.OpenSession());
        }
    }
}
