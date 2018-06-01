using Stocks.Data.Model;
using Stocks.Data.UnitTests.Model.Mocks;
using Xunit;

namespace Stocks.Data.UnitTests.Model
{
    public class CompanyTest
    {
        [Fact]
        public void CanCreateInstance()
        {
            var tested = new Company();
            tested.Ticker = "IDK";
        }
        [Fact]
        public void ToStringReturnsTicker()
        {
            const string ticker = "IDK";
            var tested = new Company {Ticker = ticker};
            Assert.Equal(ticker, tested.ToString());
        }
        [Theory]
        [ClassData(typeof(ValidCompanyProvider))]
        public void IsValidIsTrueForValid(Company inputCompany)
        {
            Assert.True(inputCompany.IsValid());
        }
        [Theory]
        [ClassData(typeof(InvalidCompanyProvider))]
        public void IsValidIsFalseForInvalid(Company inputCompany)
        {
            Assert.False(inputCompany.IsValid());
        }
    }
}
