using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore;
using Stocks.Data.Ef.Test.Mocks;
using Stocks.Data.Model;
using Xunit;

namespace Stocks.Data.Ef.Test
{
    public class RepositoryTest
    {
        [Theory]
        [ClassData(typeof(MockPocoProvider))]
        public void AddAdds(MockPoco input)
        {
            // Arrange

            var options = Config.ChoosenDbProviderFactory.GetInstance();

            DbContext testContext = null;
            Repository<MockPoco> tested = null;
            try
            {
                testContext = new MockPocoContext(options);
                tested = new Repository<MockPoco>(testContext);
                testContext.Database.EnsureCreated();

                // Act

                testContext.Add(input);
                testContext.SaveChanges();

                // Assert

                Assert.Equal(input.Value, testContext.Set<MockPoco>().First().Value);
            }
            finally
            {
                testContext?.Database.EnsureDeleted();
                tested?.Dispose();
                testContext?.Dispose();
            }
        }
        [Theory]
        [ClassData(typeof(MockPocoRangeProvider))]
        public void AddRangeAddsRange(List<MockPoco> input)
        {
            // Arrange

            var options = Config.ChoosenDbProviderFactory.GetInstance();

            DbContext testContext = null;
            Repository<MockPoco> tested = null;
            try
            {
                testContext = new MockPocoContext(options);
                tested = new Repository<MockPoco>(testContext);
                testContext.Database.EnsureCreated();

                // Act

                testContext.AddRange(input);
                testContext.SaveChanges();

                // Assert

                Assert.Equal(input.Count, testContext.Set<MockPoco>().Count());
            }
            finally
            {
                testContext?.Database.EnsureDeleted();
                tested?.Dispose();
                testContext?.Dispose();
            }
        }
        [Theory]
        [ClassData(typeof(MockPocoRangeProvider))]
        public void RemoveRangeRemovesRange(List<MockPoco> input)
        {
            // Arrange

            var options = Config.ChoosenDbProviderFactory.GetInstance();

            DbContext testContext = null;
            Repository<MockPoco> tested = null;
            try
            {
                testContext = new MockPocoContext(options);
                tested = new Repository<MockPoco>(testContext);
                testContext.Database.EnsureCreated();

                testContext.AddRange(input);
                testContext.SaveChanges();

                Assert.Equal(input.Count, testContext.Set<MockPoco>().Count());

                // Act

                tested.RemoveRange(input);
                testContext.SaveChanges();

                // Assert

                Assert.Empty(testContext.Set<MockPoco>());
            }
            finally
            {
                testContext?.Database.EnsureDeleted();
                tested?.Dispose();
                testContext?.Dispose();
            }
        }
        [Theory]
        [ClassData(typeof(MockPocoRangeProvider))]
        public void CountReturnsAccurateCount(List<MockPoco> input)
        {
            // Arrange

            var options = Config.ChoosenDbProviderFactory.GetInstance();
            var inputList = input.ToList();
            DbContext testContext = null;
            Repository<MockPoco> tested = null;

            var expected = input.Count;
            try
            {
                testContext = new MockPocoContext(options);
                tested = new Repository<MockPoco>(testContext);
                testContext.Database.EnsureCreated();

                testContext.AddRange(inputList);
                testContext.SaveChanges();

                // Act

                var actual = tested.Count();

                // Assert

                Assert.Equal(expected, actual);
            }
            finally
            {
                testContext?.Database.EnsureDeleted();
                tested?.Dispose();
                testContext?.Dispose();
            }
        }
        [Theory]
        [ClassData(typeof(MockPocoRangeProvider))]
        public void PredicateCountReturnsAccurateCount(List<MockPoco> input)
        {
            // Arrange

            var options = Config.ChoosenDbProviderFactory.GetInstance();
            var inputList = input.ToList();
            DbContext testContext = null;
            Repository<MockPoco> tested = null;
            var predicate = new Func<MockPoco, bool>(x => x.Value == "test4");
            var expected = input.Count(predicate);
            try
            {
                testContext = new MockPocoContext(options);
                tested = new Repository<MockPoco>(testContext);
                testContext.Database.EnsureCreated();

                testContext.AddRange(inputList);
                testContext.SaveChanges();

                // Act

                var actual = tested.Count(x => predicate(x));

                // Assert

                Assert.Equal(expected, actual);
            }
            finally
            {
                testContext?.Database.EnsureDeleted();
                tested?.Dispose();
                testContext?.Dispose();
            }
        }
        [Theory]
        [ClassData(typeof(MockPocoRangeProvider))]
        public void PredicateGetReturnsAccurateResult(List<MockPoco> input)
        {
            // Arrange

            var options = Config.ChoosenDbProviderFactory.GetInstance();
            var inputList = input.ToList();
            DbContext testContext = null;
            Repository<MockPoco> tested = null;
            var predicate = new Func<MockPoco, bool>(x => x.Value == "test3");
            var expected = input.FirstOrDefault(predicate);
            try
            {
                testContext = new MockPocoContext(options);
                tested = new Repository<MockPoco>(testContext);
                testContext.Database.EnsureCreated();

                // Act

                tested.AddRange(inputList);
                testContext.SaveChanges();

                // Assert
                Assert.Equal(expected, tested.Get(x => predicate(x)));
            }
            finally
            {
                testContext?.Database.EnsureDeleted();
                tested?.Dispose();
                testContext?.Dispose();
            }
        }
        [Theory]
        [ClassData(typeof(MockPocoRangeProvider))]
        public void PredicateGetAllReturnsAccurateResult(List<MockPoco> input)
        {
            // Arrange

            var options = Config.ChoosenDbProviderFactory.GetInstance();
            var inputList = input.ToList();
            DbContext testContext = null;
            Repository<MockPoco> tested = null;
            var predicate = new Func<MockPoco, bool>(x => x.Value == "test3");
            var expected = input.Where(predicate);
            try
            {
                testContext = new MockPocoContext(options);
                tested = new Repository<MockPoco>(testContext);
                testContext.Database.EnsureCreated();

                testContext.AddRange(inputList);
                testContext.SaveChanges();

                // Act

                var actual = tested.GetAll(x => predicate(x));

                // Assert

                Assert.Equal(expected, actual);
            }
            finally
            {
                testContext?.Database.EnsureDeleted();
                tested?.Dispose();
                testContext?.Dispose();
            }
        }
        [Theory]
        [ClassData(typeof(MockPocoRangeProvider))]
        public void PredicateRemoveAllRemoves(List<MockPoco> input)
        {
            // Arrange

            var options = Config.ChoosenDbProviderFactory.GetInstance();
            var inputList = input.ToList();
            DbContext testContext = null;
            Repository<MockPoco> tested = null;
            var predicate = new Func<MockPoco, bool>(x => x.Value == "test3");
            var except = input.Where(predicate);
            var expected = input.Except(except);
            try
            {
                testContext = new MockPocoContext(options);
                tested = new Repository<MockPoco>(testContext);
                testContext.Database.EnsureCreated();
                testContext.AddRange(inputList);
                testContext.SaveChanges();

                // Act

                tested.RemoveAll(x => predicate(x));
                testContext.SaveChanges();

                // Assert

                Assert.Equal(expected, testContext.Set<MockPoco>());
            }
            finally
            {
                testContext?.Database.EnsureDeleted();
                tested?.Dispose();
                testContext?.Dispose();
            }
        }
        [Theory]
        [ClassData(typeof(MockPocoProvider))]
        public void AddOrUpdateAdds(MockPoco input)
        {
            // Arrange

            var options = Config.ChoosenDbProviderFactory.GetInstance();
            DbContext testContext = null;
            Repository<MockPoco> tested = null;
            try
            {
                testContext = new MockPocoContext(options);
                tested = new Repository<MockPoco>(testContext);
                testContext.Database.EnsureCreated();

                // Act

                tested.AddOrUpdate(input);
                testContext.SaveChanges();

                // Assert

                Assert.Equal(input, testContext.Set<MockPoco>().First());
            }
            finally
            {
                testContext?.Database.EnsureDeleted();
                tested?.Dispose();
                testContext?.Dispose();
            }
        }
        [Theory]
        [ClassData(typeof(MockPocoProvider))]
        public void AddOrUpdateUpdates(MockPoco input)
        {
            // Arrange
            const string expected = "newValue1";
            var options = Config.ChoosenDbProviderFactory.GetInstance();
            DbContext testContext = null;
            Repository<MockPoco> tested = null;
            try
            {
                testContext = new MockPocoContext(options);
                tested = new Repository<MockPoco>(testContext);
                testContext.Database.EnsureCreated();
                testContext.Add(input);
                testContext.SaveChanges();

                // Act

                input.Value = expected;
                tested.AddOrUpdate(input);
                testContext.SaveChanges();

                // Assert

                Assert.Equal(input.Value, testContext.Set<MockPoco>().First().Value);
            }
            finally
            {
                testContext?.Database.EnsureDeleted();
                tested?.Dispose();
                testContext?.Dispose();
            }
        }
    }
}
