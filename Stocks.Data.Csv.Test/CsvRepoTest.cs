using Stocks.Data.Csv.Test.Mocks;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using Xunit;

namespace Stocks.Data.Csv.Test
{
    public class CsvRepoTest
    {
        [Theory]
        [ClassData(typeof(MockPocoProvider))]
        public void AddToCsvRepoWorks(MockPoco input)
        {
            // Arrange

            var fileName = Path.GetRandomFileName();
            var outputFile = new FileInfo(Path.ChangeExtension(fileName, "csv"));
            CsvContext<MockPoco> csvContext = null;
            CsvRepo<MockPoco> repository = null;
            try
            {
                //outputFile.Create();

                csvContext = new CsvContext<MockPoco>(outputFile);
                //Thread.Sleep(3000);
                repository = new CsvRepo<MockPoco>(csvContext);

                // Act

                repository.Add(input);
                csvContext.SaveChanges();

                var received = File.ReadAllText(outputFile.FullName);

                // Assert

                Assert.Contains(input.Value, received);
            }
            finally
            {
                repository?.Dispose();
                outputFile.Delete();
            }
        }
    }
}
