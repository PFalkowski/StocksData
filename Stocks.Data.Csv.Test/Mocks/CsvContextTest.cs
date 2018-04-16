using Stocks.Data.Model;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Text;
using Xunit;

namespace Stocks.Data.Csv.Test.Mocks
{
    public class CsvContextTest
    {
        [Theory]
        [ClassData(typeof(_11BitMock))]
        public void AddStock(Company company)
        {
            // Arrange

            var fileName = Path.GetRandomFileName();
            var outputFile = new FileInfo(Path.ChangeExtension(fileName, "csv"));
            CsvContext<StockQuote> csvContext = null;
            CsvRepo<StockQuote> repository = null;
            try
            {
                csvContext = new StockCsvContext(outputFile) { Culture = CultureInfo.InvariantCulture };
                repository = new CsvRepo<StockQuote>(csvContext);

                // Act

                repository.AddRange(company.Quotes);
                csvContext.SaveChanges();

                var received = File.ReadAllText(outputFile.FullName);
                // Assert

                var actual = received.Split(Environment.NewLine).Length;
                var expected = company.Quotes.Count + 2;
                Assert.Equal(expected, actual);
            }
            finally
            {
                repository?.Dispose();
                outputFile.Delete();
            }
        }
    }
}
