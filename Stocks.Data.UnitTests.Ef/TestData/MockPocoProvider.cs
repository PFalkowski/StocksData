using Xunit;

namespace Stocks.Data.UnitTests.Ef.Test.TestData
{
    public class MockPocoProvider : TheoryData<MockPoco>
    {
        public MockPocoProvider()
        {
            Add(new MockPoco{Value = "test1"});
            Add(new MockPoco{Value = "test2"});
            Add(new MockPoco{Value = "test3"});
            Add(new MockPoco{Value = "test1" });
            Add(new MockPoco{Value = "test2" });
        }
    }
}
