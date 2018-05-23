using System;
using Stocks.Data.Csv.Test.Mocks;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using CsvHelper;
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
                csvContext = new CsvContext<MockPoco>(outputFile) { Culture = CultureInfo.InvariantCulture };
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
                csvContext = new CsvContext<MockPoco>(outputFile) { Culture = CultureInfo.InvariantCulture };
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
                csvContext = new CsvContext<MockPoco>(outputFile) { Culture = CultureInfo.InvariantCulture };
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
                csvContext = new CsvContext<MockPoco>(outputFile) { Culture = CultureInfo.InvariantCulture };
                repository = new CsvRepo<MockPoco>(csvContext);

                // Act

                repository.GetAll().First().Value = input.Value;
                csvContext.SaveChanges();

                var received = File.ReadAllText(outputFile.FullName);

                // Assert

                Assert.DoesNotContain("test5", received);
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
        public void CsvRepRemoveRemoves(MockPoco input)
        {
            // Arrange

            var fileName = Path.GetRandomFileName();
            var outputFile = new FileInfo(Path.ChangeExtension(fileName, "csv"));

            CsvContext<MockPoco> csvContext = null;
            CsvRepo<MockPoco> repository = null;
            try
            {
                csvContext = new CsvContext<MockPoco>(outputFile) { Culture = CultureInfo.InvariantCulture };
                repository = new CsvRepo<MockPoco>(csvContext);

                csvContext.Entities.Add(input);
                csvContext.SaveChanges();

                // Act

                repository.Remove(input);
                csvContext.SaveChanges();
                var received = File.ReadAllText(outputFile.FullName);

                // Assert

                Assert.DoesNotContain(input.Value, received);
            }
            finally
            {
                repository?.Dispose();

                outputFile.Delete();
            }
        }
        [Fact]
        public void AddThrowsWhenAttemptedToAddExisitngId()
        {
            // Arrange

            var fileName = Path.GetRandomFileName();
            var outputFile = new FileInfo(Path.ChangeExtension(fileName, "csv"));

            CsvContext<MockPoco> csvContext = null;
            CsvRepo<MockPoco> repository = null;
            //
            var set = new HashSet<int>();
            set.Add(1);
            set.Add(1);
            //
            using (var streamWriter = outputFile.CreateText())
            {
                streamWriter.Write("Id,Value\r\n5,test5\r\n");
            }

            try
            {
                csvContext = new CsvContext<MockPoco>(outputFile) { Culture = CultureInfo.InvariantCulture };
                repository = new CsvRepo<MockPoco>(csvContext);

                // Act & Assert

                Assert.Throws<ArgumentException>(() => repository.Add(new MockPoco { Id = 5 }));
            }
            finally
            {
                repository?.Dispose();

                outputFile.Delete();
            }
        }
        [Theory]
        [ClassData(typeof(MockPocoRangeProvider))]
        public void AddRangeAddsRange(List<MockPoco> input)
        {
            // Arrange

            var fileName = Path.GetRandomFileName();
            var outputFile = new FileInfo(Path.ChangeExtension(fileName, "csv"));

            CsvContext<MockPoco> csvContext = null;
            CsvRepo<MockPoco> repository = null;
            try
            {
                csvContext = new CsvContext<MockPoco>(outputFile) { Culture = CultureInfo.InvariantCulture };
                repository = new CsvRepo<MockPoco>(csvContext);

                // Act

                repository.AddRange(input);
                csvContext.SaveChanges();

                // Assert

                var rawAllText = File.ReadAllText(outputFile.FullName);
                List<MockPoco> fromFileParsed = null;
                using (var csv = new CsvReader(new StringReader(rawAllText)))
                {
                    fromFileParsed = csv.GetRecords<MockPoco>().ToList();
                }

                var expected = input.GroupBy(x => x.Value).OrderBy(x => x.Key).Select(g => new { g.Key, Count = g.Count() });
                var actual = fromFileParsed.GroupBy(x => x.Value).OrderBy(x => x.Key).Select(g => new { g.Key, Count = g.Count() });

                Assert.Equal(expected, actual);
            }
            finally
            {
                repository?.Dispose();

                outputFile.Delete();
            }
        }
        [Theory]
        [ClassData(typeof(MockPocoRangeProvider))]
        public void RemoveRangeRemovesRange(List<MockPoco> input)
        {
            // Arrange

            var fileName = Path.GetRandomFileName();
            var outputFile = new FileInfo(Path.ChangeExtension(fileName, "csv"));

            CsvContext<MockPoco> csvContext = null;
            CsvRepo<MockPoco> repository = null;
            try
            {
                csvContext = new CsvContext<MockPoco>(outputFile) { Culture = CultureInfo.InvariantCulture };
                repository = new CsvRepo<MockPoco>(csvContext);
                foreach (var entity in input)
                {
                    csvContext.Entities.Add(entity);
                }

                // Act

                repository.RemoveRange(input);
                csvContext.SaveChanges();


                // Assert

                var rawAllText = File.ReadAllText(outputFile.FullName);
                List<MockPoco> fromFileParsed = null;
                using (var csv = new CsvReader(new StringReader(rawAllText)))
                {
                    fromFileParsed = csv.GetRecords<MockPoco>().ToList();
                }

                var actual = fromFileParsed.GroupBy(x => x.Value).OrderBy(x => x.Key).Select(g => new { g.Key, Count = g.Count() }).ToList();

                Assert.Empty(actual);
            }
            finally
            {
                repository?.Dispose();

                outputFile.Delete();
            }
        }
        [Theory]
        [ClassData(typeof(MockPocoRangeProvider))]
        public void RemoveRangeRemovesRangeWhenTheSameCollection(List<MockPoco> input)
        {
            // Arrange

            var fileName = Path.GetRandomFileName();
            var outputFile = new FileInfo(Path.ChangeExtension(fileName, "csv"));

            CsvContext<MockPoco> csvContext = null;
            CsvRepo<MockPoco> repository = null;
            try
            {
                csvContext = new CsvContext<MockPoco>(outputFile) { Culture = CultureInfo.InvariantCulture };
                repository = new CsvRepo<MockPoco>(csvContext);

                // Act

                repository.AddRange(input);
                var unfortunateReference = repository.GetAll();
                repository.RemoveRange(unfortunateReference);
                csvContext.SaveChanges();


                // Assert

                var rawAllText = File.ReadAllText(outputFile.FullName);
                List<MockPoco> fromFileParsed = null;
                using (var csv = new CsvReader(new StringReader(rawAllText)))
                {
                    fromFileParsed = csv.GetRecords<MockPoco>().ToList();
                }

                var actual = fromFileParsed.GroupBy(x => x.Value).OrderBy(x => x.Key).Select(g => new { g.Key, Count = g.Count() }).ToList();

                Assert.Empty(actual);
            }
            finally
            {
                repository?.Dispose();

                outputFile.Delete();
            }
        }

        [Theory]
        [ClassData(typeof(MockPocoProvider))]
        public void AddOrUpdateUpdates(MockPoco input)
        {
            string oldValue = input.Value;
            const string newValue = "newValue5";
            // Arrange

            var fileName = Path.GetRandomFileName();
            var outputFile = new FileInfo(Path.ChangeExtension(fileName, "csv"));


            using (var streamWriter = outputFile.CreateText())
            {
                streamWriter.Write($"Id,Value\r\n{input.Id},{input.Value}\r\n");
            }
            CsvContext<MockPoco> csvContext = null;
            CsvRepo<MockPoco> repository = null;
            try
            {
                csvContext = new CsvContext<MockPoco>(outputFile) { Culture = CultureInfo.InvariantCulture };
                repository = new CsvRepo<MockPoco>(csvContext);
                csvContext.SaveChanges();

                // Act
                input.Value = newValue;
                repository.AddOrUpdate(input);
                csvContext.SaveChanges();

                var received = File.ReadAllText(outputFile.FullName);

                // Assert

                Assert.Contains(newValue, received);
                Assert.DoesNotContain(oldValue, received);
            }
            finally
            {
                repository?.Dispose();

                outputFile.Delete();
            }
        }
        [Theory]
        [ClassData(typeof(MockPocoProvider))]
        public void UpdateUpdates(MockPoco input)
        {
            string oldValue = input.Value;
            const string newValue = "newValue5";
            // Arrange

            var fileName = Path.GetRandomFileName();
            var outputFile = new FileInfo(Path.ChangeExtension(fileName, "csv"));

            CsvContext<MockPoco> csvContext = null;
            CsvRepo<MockPoco> repository = null;
            try
            {
                csvContext = new CsvContext<MockPoco>(outputFile) { Culture = CultureInfo.InvariantCulture };
                repository = new CsvRepo<MockPoco>(csvContext);

                // Act
                repository.Add(input);
                input.Value = newValue;
                csvContext.SaveChanges();

                var received = File.ReadAllText(outputFile.FullName);

                // Assert

                Assert.Contains(newValue, received);
                Assert.DoesNotContain(oldValue, received);
            }
            finally
            {
                repository?.Dispose();

                outputFile.Delete();
            }
        }
        [Theory]
        [ClassData(typeof(MockPocoProvider))]
        public void UpdateUpdatesAfterChangesCommited(MockPoco input)
        {
            string oldValue = input.Value;
            const string newValue = "newValue5";
            // Arrange

            var fileName = Path.GetRandomFileName();
            var outputFile = new FileInfo(Path.ChangeExtension(fileName, "csv"));

            CsvContext<MockPoco> csvContext = null;
            CsvRepo<MockPoco> repository = null;
            try
            {
                csvContext = new CsvContext<MockPoco>(outputFile) { Culture = CultureInfo.InvariantCulture };
                repository = new CsvRepo<MockPoco>(csvContext);

                // Act
                repository.Add(input);
                csvContext.SaveChanges();
                input.Value = newValue;
                csvContext.SaveChanges();

                var received = File.ReadAllText(outputFile.FullName);

                // Assert

                Assert.Contains(newValue, received);
                Assert.DoesNotContain(oldValue, received);
            }
            finally
            {
                repository?.Dispose();

                outputFile.Delete();
            }
        }
        [Theory]
        [ClassData(typeof(MockPocoProvider))]
        public void GetGets(MockPoco input)
        {
            // Arrange

            var fileName = Path.GetRandomFileName();
            var outputFile = new FileInfo(Path.ChangeExtension(fileName, "csv"));

            CsvContext<MockPoco> csvContext = null;
            CsvRepo<MockPoco> repository = null;
            try
            {
                csvContext = new CsvContext<MockPoco>(outputFile) { Culture = CultureInfo.InvariantCulture };
                repository = new CsvRepo<MockPoco>(csvContext);
                csvContext.Entities.Add(input);
                csvContext.SaveChanges();

                // Act
                var received = repository.Get(input);

                // Assert

                Assert.Equal(input, received);
            }
            finally
            {
                repository?.Dispose();

                outputFile.Delete();
            }
        }
    }
}
