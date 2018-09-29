using Stocks.Data.Model;

namespace Services
{
    public interface IStocksDeserializer
    {
        Company Deserialize(string fileContents);
    }
}