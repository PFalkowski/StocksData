using System;
using Stocks.Data.Model;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text.RegularExpressions;
using LoggerLite;
using Stocks.Data.Infrastructure;

namespace Stocks.Data.Ado
{
    public class CompanyBulkInserter : BulkInserter<Company>
    {
        private readonly BulkInserter<StockQuote> _stockQuoteBulkInserter;
        private readonly ILogger _logger;

        public CompanyBulkInserter(BulkInserter<StockQuote> stockQuoteBulkInserter, ILogger logger)
        {
            _stockQuoteBulkInserter = stockQuoteBulkInserter;
            _logger = logger;
        }

        public override void BulkInsert(string connectionString, string destinationTableName, IEnumerable<Company> companies)
        {

            using (var connection = new SqlConnection(connectionString))
            {
                var blacklistPattern = new Regex(Constants.BlacklistPatternString, RegexOptions.Compiled);
                connection.Open();
                foreach (var company in companies.Where(x => !blacklistPattern.IsMatch(x.Ticker)))
                {
                    using (var command = new SqlCommand($"if not exists" +
                                                        $" (select * from [{Constants.CompanyName}] where {Constants.TickerName} = @value)" +
                                                        $" begin insert into [{Constants.CompanyName}] ({Constants.TickerName}) values (@value) end",
                        connection))
                    {
                        command.Parameters.Add("@value", SqlDbType.VarChar);
                        command.Parameters["@value"].Value = company.Ticker;
                        command.ExecuteNonQuery();
                    }

                    try
                    {
                        _stockQuoteBulkInserter.BulkInsert(connectionString, Constants.StockQuoteName, company.Quotes);
                    }
                    catch (Exception e)
                    {
                        _logger.LogError($"{e.Message} - rolling back all data for {company.Ticker}. {company.Ticker} will be skipped");
                    }
                }
            }
        }
    }
}
