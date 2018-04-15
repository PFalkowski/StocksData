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
        public void AddToCsvRepoWorksWhenFileDoesNotExist(MockPoco input)
        {
            // Arrange

            var fileName = Path.GetRandomFileName();
            var outputFile = new FileInfo(Path.ChangeExtension(fileName, "csv"));
            CsvContext<MockPoco> csvContext = null;
            CsvRepo<MockPoco> repository = null;
            try
            {
                csvContext = new CsvContext<MockPoco>(outputFile);
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
        [Theory]
        [ClassData(typeof(MockPocoProvider))]
        public void AddToCsvRepoWorksWhenEmptyFileExists(MockPoco input)
        {
            // Arrange

            var fileName = Path.GetRandomFileName();
            var outputFile = new FileInfo(Path.ChangeExtension(fileName, "csv"));
            outputFile.Create().Dispose();
            CsvContext<MockPoco> csvContext = null;
            CsvRepo<MockPoco> repository = null;
            try
            {
                csvContext = new CsvContext<MockPoco>(outputFile);
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
        [Theory]
        [ClassData(typeof(MockPocoProvider))]
        public void AddToCsvRepoWorksWhenNonEmptyFileExists(MockPoco input)
        {
            // Arrange

            var fileName = Path.GetRandomFileName();
            var outputFile = new FileInfo(Path.ChangeExtension(fileName, "csv"));


            using (var streamWriter = outputFile.CreateText())
            {
                streamWriter.Write("Id,Value\r\n5,test5\r\n");
            }
            CsvContext<MockPoco> csvContext = null;
            CsvRepo<MockPoco> repository = null;
            try
            {
                csvContext = new CsvContext<MockPoco>(outputFile);
                repository = new CsvRepo<MockPoco>(csvContext);

                // Act

                repository.Add(input);
                csvContext.SaveChanges();

                var received = File.ReadAllText(outputFile.FullName);

                // Assert

                Assert.Contains("test5", received);
                Assert.Contains(input.Value, received);
            }
            finally
            {
                repository?.Dispose();
                outputFile.Delete();
            }
        }
        [Theory]
        [ClassData(typeof(MockPocoProvider))]
        public void CsvRepTracksChangesOnItemsInContext(MockPoco input)
        {
            const string newValue = "newTest123";
            // Arrange

            var fileName = Path.GetRandomFileName();
            var outputFile = new FileInfo(Path.ChangeExtension(fileName, "csv"));


            using (var streamWriter = outputFile.CreateText())
            {
                streamWriter.Write("Id,Value\r\n5,test5\r\n");
            }
            CsvContext<MockPoco> csvContext = null;
            CsvRepo<MockPoco> repository = null;
            try
            {
                csvContext = new CsvContext<MockPoco>(outputFile);
                repository = new CsvRepo<MockPoco>(csvContext);

                // Act

                csvContext.Entities.First().Value = newValue;
                csvContext.SaveChanges();

                var received = File.ReadAllText(outputFile.FullName);

                // Assert

                Assert.DoesNotContain("test5", received);
                Assert.Contains(newValue, received);
            }
            finally
            {
                repository?.Dispose();
                outputFile.Delete();
            }
        }
    }
}
