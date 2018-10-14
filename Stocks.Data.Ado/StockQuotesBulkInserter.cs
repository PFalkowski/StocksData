using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using Stocks.Data.Model;
using Stocks.Data.Infrastructure;

namespace Stocks.Data.Ado
{
    public class StockQuotesBulkInserter : BulkInserter<StockQuote>
    {

        public StockQuotesBulkInserter(string connectionString)
        {
            ConnectionString = connectionString;
        }

        public override void BulkInsert(string destinationTableName, IEnumerable<StockQuote> quotes)
        {
            var sqlConnection = default(SqlConnection);
            var sqlBulkCopy = default(SqlBulkCopy);
            var inMemoryTable = default(DataTable);
            try
            {
                sqlConnection = new SqlConnection(ConnectionString);
                sqlBulkCopy = new SqlBulkCopy(sqlConnection)
                {
                    DestinationTableName = destinationTableName,
                    ColumnMappings =
                    {
                        new SqlBulkCopyColumnMapping(Constants.TickerName, Constants.TickerName),
                        new SqlBulkCopyColumnMapping(Constants.DateName, Constants.DateName),
                        new SqlBulkCopyColumnMapping(Constants.OpenName, Constants.OpenName),
                        new SqlBulkCopyColumnMapping(Constants.HighName, Constants.HighName),
                        new SqlBulkCopyColumnMapping(Constants.LowName, Constants.LowName),
                        new SqlBulkCopyColumnMapping(Constants.CloseName, Constants.CloseName),
                        new SqlBulkCopyColumnMapping(Constants.VolName, Constants.VolName)
                    }
                };
                inMemoryTable = new DataTable(destinationTableName)
                {
                    Columns = {
                        new DataColumn(Constants.TickerName, typeof(string)),
                        new DataColumn(Constants.DateName, typeof(int)),
                        new DataColumn(Constants.OpenName, typeof(double)),
                        new DataColumn(Constants.HighName, typeof(double)),
                        new DataColumn(Constants.LowName, typeof(double)),
                        new DataColumn(Constants.CloseName, typeof(double)),
                        new DataColumn(Constants.VolName, typeof(double))
                    }
                };
                inMemoryTable.PrimaryKey = new[]
                {
                    inMemoryTable.Columns[0],
                    inMemoryTable.Columns[1]
                };

                foreach (var quote in quotes)
                {
                    var newQuoteRow = inMemoryTable.NewRow();

                    newQuoteRow[Constants.TickerName] = quote.Ticker;
                    newQuoteRow[Constants.DateName] = quote.Date;
                    newQuoteRow[Constants.OpenName] = quote.Open;
                    newQuoteRow[Constants.HighName] = quote.High;
                    newQuoteRow[Constants.LowName] = quote.Low;
                    newQuoteRow[Constants.CloseName] = quote.Close;
                    newQuoteRow[Constants.VolName] = quote.Volume;

                    inMemoryTable.Rows.Add(newQuoteRow);
                }
                sqlConnection.Open();
                sqlBulkCopy.WriteToServer(inMemoryTable);
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }
            finally
            {
                sqlConnection?.Dispose();
                inMemoryTable?.Dispose();
                ((IDisposable)sqlBulkCopy)?.Dispose();
            }
        }
    }
}
