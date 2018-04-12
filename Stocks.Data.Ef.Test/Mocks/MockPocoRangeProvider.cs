using System.Collections;
using System.Collections.Generic;
using Stocks.Data.Model;
using Xunit;

namespace Stocks.Data.Ef.Test.Mocks
{
    public class MockPocoRangeProvider : TheoryData<IEnumerable<MockPoco>>
    {
        public MockPocoRangeProvider()
        {
            Add(new List<MockPoco>{ new MockPoco { Value = "test1" }, new MockPoco { Value = "test2" }, new MockPoco { Value = "test1" }, new MockPoco { Value = "test2" } });
        }
    }
}
