using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using LoggerLite;
using Stocks.Data.Model;
using Stocks.Data.Infrastructure;

namespace Stocks.Data.Ado
{
    public class StockQuotesBulkInserter : BulkInserter<StockQuote>
    {
        private readonly ILogger _logger;

        public StockQuotesBulkInserter(ILogger logger)
        {
            _logger = logger;
        }

        public override void BulkInsert(string connectionString, string destinationTableName, IEnumerable<StockQuote> quotes)
        {
            var sqlConnection = default(SqlConnection);
            var sqlBulkCopy = default(SqlBulkCopy);
            var inMemoryTable = default(DataTable);
            try
            {
                sqlConnection = new SqlConnection(connectionString);
                sqlBulkCopy = new SqlBulkCopy(sqlConnection)
                {
                    DestinationTableName = destinationTableName,
                    ColumnMappings =
                    {
                        new SqlBulkCopyColumnMapping(Constants.TickerName, Constants.TickerName),
                        new SqlBulkCopyColumnMapping(Constants.DateName, Constants.DateName),
                        new SqlBulkCopyColumnMapping(Constants.PreviousStockQuoteTickerName, Constants.PreviousStockQuoteTickerName),
                        new SqlBulkCopyColumnMapping(Constants.PreviousStockQuoteDateName, Constants.PreviousStockQuoteDateName),
                        new SqlBulkCopyColumnMapping(Constants.OpenName, Constants.OpenName),
                        new SqlBulkCopyColumnMapping(Constants.HighName, Constants.HighName),
                        new SqlBulkCopyColumnMapping(Constants.LowName, Constants.LowName),
                        new SqlBulkCopyColumnMapping(Constants.CloseName, Constants.CloseName),
                        new SqlBulkCopyColumnMapping(Constants.VolName, Constants.VolName),
                        new SqlBulkCopyColumnMapping(Constants.TotalSharesEmittedName, Constants.TotalSharesEmittedName),
                        new SqlBulkCopyColumnMapping(Constants.MarketCapName, Constants.MarketCapName),
                        new SqlBulkCopyColumnMapping(Constants.BookValueName, Constants.BookValueName),
                        new SqlBulkCopyColumnMapping(Constants.DividendYieldName, Constants.DividendYieldName),
                        new SqlBulkCopyColumnMapping(Constants.PriceToEarningsRatioName, Constants.PriceToEarningsRatioName),
                    }
                };
                inMemoryTable = new DataTable(destinationTableName)
                {
                    Columns = {
                        new DataColumn(Constants.TickerName, typeof(string)),
                        new DataColumn(Constants.DateName, typeof(int)),
                        new DataColumn(Constants.PreviousStockQuoteTickerName, typeof(string)),
                        new DataColumn(Constants.PreviousStockQuoteDateName, typeof(int)),
                        new DataColumn(Constants.OpenName, typeof(double)),
                        new DataColumn(Constants.HighName, typeof(double)),
                        new DataColumn(Constants.LowName, typeof(double)),
                        new DataColumn(Constants.CloseName, typeof(double)),
                        new DataColumn(Constants.VolName, typeof(double)),
                        new DataColumn(Constants.TotalSharesEmittedName, typeof(long)) { AllowDBNull = true },
                        new DataColumn(Constants.MarketCapName, typeof(double)) { AllowDBNull = true },
                        new DataColumn(Constants.BookValueName, typeof(double)) { AllowDBNull = true },
                        new DataColumn(Constants.DividendYieldName, typeof(double)) { AllowDBNull = true },
                        new DataColumn(Constants.PriceToEarningsRatioName, typeof(double)) { AllowDBNull = true },
                    }
                };
                inMemoryTable.PrimaryKey = new[]
                {
                    inMemoryTable.Columns[Constants.TickerName],
                    inMemoryTable.Columns[Constants.DateName]
                };
                
                StockQuote previous = null;
                foreach (var quote in quotes)
                {
                    var newQuoteRow = inMemoryTable.NewRow();
                    
                    if (previous != null && previous.Ticker.Equals(quote.Ticker))
                    {
                        newQuoteRow[Constants.PreviousStockQuoteTickerName] = previous.Ticker;
                        newQuoteRow[Constants.PreviousStockQuoteDateName] = previous.Date;
                    }
                    newQuoteRow[Constants.TickerName] = quote.Ticker;
                    newQuoteRow[Constants.DateName] = quote.Date;
                    newQuoteRow[Constants.OpenName] = quote.Open;
                    newQuoteRow[Constants.HighName] = quote.High;
                    newQuoteRow[Constants.LowName] = quote.Low;
                    newQuoteRow[Constants.CloseName] = quote.Close;
                    newQuoteRow[Constants.VolName] = quote.Volume;
                    newQuoteRow[Constants.TotalSharesEmittedName] = DBNull.Value;
                    newQuoteRow[Constants.MarketCapName] = DBNull.Value;
                    newQuoteRow[Constants.BookValueName] = DBNull.Value;
                    newQuoteRow[Constants.DividendYieldName] = DBNull.Value;
                    newQuoteRow[Constants.PriceToEarningsRatioName] = DBNull.Value;
                    
                    if (previous != null && previous.Ticker.Equals(quote.Ticker) && previous.DateParsed > quote.DateParsed)
                    {
                        throw new ArgumentException(nameof(quotes));
                    }
                    if (previous == null || previous?.Ticker != null && quote.Ticker.Equals(previous.Ticker))
                    {
                        previous = quote;
                    }
                    try
                    {
                        inMemoryTable.Rows.Add(newQuoteRow);
                    }
                    catch (ConstraintException constraintException)
                    {
                        _logger?.LogWarning($"{constraintException.Message}. Skipped.");
                    }
                }
                sqlConnection.Open();
                sqlBulkCopy.WriteToServer(inMemoryTable);
            }
            catch (Exception e)
            {
                _logger?.LogError(e);
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
