using System;
using System.Collections.Generic;
using System.Text;
using Stocks.Data.Model.Test.Mocks;
using Xunit;

namespace Stocks.Data.Model.Test
{
    public class CompanyTest
    {
        [Fact]
        public void CanCreateInstance()
        {
            var tested = new Company();
            tested.Ticker = "IDK";
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
