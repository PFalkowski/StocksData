using System.Collections.Generic;
using Xunit;

namespace Stocks.Data.Csv.Test.Mocks
{
    public class MockPocoRangeProvider : TheoryData<List<MockPoco>>
    {
        public MockPocoRangeProvider()
        {
            Add(new List<MockPoco>{ new MockPoco { Id=0, Value = "test1" }, new MockPoco { Id = 1, Value = "test2" }, new MockPoco { Id = 2, Value = "test1" }, new MockPoco { Id = 3, Value = "test2" }, new MockPoco { Id = 4, Value = "test4" }, new MockPoco { Id = 5, Value = "test2" } });
            Add(new List<MockPoco>{ new MockPoco { Id = 0, Value = "test3" }, new MockPoco { Id = 1, Value = "test2" }, new MockPoco { Id = 2, Value = "test3" }, new MockPoco { Id = 3, Value = "test3" }, new MockPoco { Id = 4, Value = "test3" }, new MockPoco { Id = 5, Value = "test2" }, new MockPoco { Id = 6, Value = "test7" } });
        }
    }
}
