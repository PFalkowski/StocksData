using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using CsvHelper.Configuration;
using Stocks.Data.Model;

namespace Stocks.Data.Csv
{
    public class StockCsvContext : CsvContext<StockQuote>
    {
        public StockCsvContext(FileInfo file) : base(file)
        {
            CustomMapping = new StockQuoteCsvClassMap();
        }

        public StockCsvContext(string filePath) : this(new FileInfo(filePath))
        {
        }
    }
}
