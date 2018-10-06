using System.Collections.Generic;
using Xunit;

namespace Stocks.Data.UnitTests.Ef.Test.TestData
{
    public class MockPocoRangeProvider : TheoryData<List<MockPoco>>
    {
        public MockPocoRangeProvider()
        {
            Add(new List<MockPoco>{ new MockPoco { Value = "test1" }, new MockPoco { Value = "test2" }, new MockPoco { Value = "test1" }, new MockPoco { Value = "test2" }, new MockPoco { Value = "test4" }, new MockPoco { Value = "test2" } });
            Add(new List<MockPoco>{ new MockPoco { Value = "test3" }, new MockPoco { Value = "test2" }, new MockPoco { Value = "test3" }, new MockPoco { Value = "test3" }, new MockPoco { Value = "test3" }, new MockPoco { Value = "test2" }, new MockPoco { Value = "test7" } });
        }
    }
}
