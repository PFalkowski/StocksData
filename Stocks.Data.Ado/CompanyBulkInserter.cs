using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using Stocks.Data.Model;

namespace Stocks.Data.Ado
{
    public class CompanyBulkInserter : BulkInserter<Company>
    {
        public BulkInserter<StockQuote> StockQuoteBulkInserter { get; set; }

        public override void BulkInsert(string connectionString, string destinationTableName, IEnumerable<Company> companies)
        {

            using (var connection = new SqlConnection(connectionString))
            {
                connection.Open();
                foreach (var company in companies)
                {
                    using (var command = new SqlCommand("insert into [Companies] (Ticker) values (@value)", connection))
                    {
                        command.Parameters.Add("@value", SqlDbType.VarChar);
                        command.Parameters["@value"].Value = company.Ticker;
                        command.ExecuteNonQuery();
                    }
                    StockQuoteBulkInserter.BulkInsert(connectionString, destinationTableName, company.Quotes);
                }
            }
        }
    }
}
