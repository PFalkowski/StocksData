using System.Threading.Tasks;
using Stocks.Data.Model;

namespace Stocks.Data.Services
{
    public interface IStocksDeserializer
    {
        Company Deserialize(string fileContents);
        Task<Company> DeserializeAsync(string fileContents);
    }
}