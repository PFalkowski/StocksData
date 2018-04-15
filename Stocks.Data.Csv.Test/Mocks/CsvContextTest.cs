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
            CsvContext<Company> csvContext = null;
            CsvRepo<Company> repository = null;
            try
            {
                csvContext = new CsvContext<Company>(outputFile) { Culture = CultureInfo.InvariantCulture };
                repository = new CsvRepo<Company>(csvContext);

                // Act

                repository.Add(company);
                csvContext.SaveChanges();

                var received = File.ReadAllText(outputFile.FullName);

                // Assert

                //Assert.Contains(input.Value, received);
            }
            finally
            {
                repository?.Dispose();
                outputFile.Delete();
            }
        }
    }
}
