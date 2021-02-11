using Stocks.Data.Model;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;

namespace Stocks.Data.Ado
{
    public class CompanyBulkInserter : BulkInserter<Company>
    {
        private readonly BulkInserter<StockQuote> _stockQuoteBulkInserter;

        public CompanyBulkInserter(BulkInserter<StockQuote> stockQuoteBulkInserter)
        {
            _stockQuoteBulkInserter = stockQuoteBulkInserter;
        }

        public override void BulkInsert(string connectionString, string destinationTableName, IEnumerable<Company> companies)
        {

            using (var connection = new SqlConnection(connectionString))
            {
                connection.Open();
                foreach (var company in companies)
                {
                    using (var command = new SqlCommand($"if not exists" +
                                                        $" (select * from [{Infrastructure.Constants.CompanyName}] where {Infrastructure.Constants.TickerName} = @value)" +
                                                        $" begin insert into [{Infrastructure.Constants.CompanyName}] ({Infrastructure.Constants.TickerName}) values (@value) end",
                        connection))
                    {
                        command.Parameters.Add("@value", SqlDbType.VarChar);
                        command.Parameters["@value"].Value = company.Ticker;
                        command.ExecuteNonQuery();
                    }
                    _stockQuoteBulkInserter.BulkInsert(connectionString, Infrastructure.Constants.StockQuoteName, company.Quotes);
                }
            }
        }
    }
}
