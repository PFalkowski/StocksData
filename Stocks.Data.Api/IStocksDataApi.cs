using System.Threading.Tasks;

namespace Stocks.Data.Api
{
    public interface IStocksDataApi
    {
        Task Execute(params string[] args);
    }
}