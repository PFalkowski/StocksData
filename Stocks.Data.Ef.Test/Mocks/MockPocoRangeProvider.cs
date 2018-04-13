using System.Collections;
using System.Collections.Generic;
using Stocks.Data.Model;
using Xunit;

namespace Stocks.Data.Ef.Test.Mocks
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
