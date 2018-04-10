using System.Linq;
using Microsoft.EntityFrameworkCore;
using Stocks.Data.Ef.Test.Mocks;
using Stocks.Data.Model;
using Stocks.Data.Model.Test.Mocks;
using Xunit;

namespace Stocks.Data.Ef.Test
{
    public class RepositoryTest
    {
        [Theory]
        [ClassData(typeof(_11BitMock))]
        [ClassData(typeof(CdProjectMock))]
        [ClassData(typeof(MbankMock))]
        public void AddStockInMemory(Company company)
        {
            var options = new DbContextOptionsBuilder<StockContext>()
                .UseInMemoryDatabase(databaseName: nameof(AddStockInMemory))
                .Options;

            StockContext testContext = null;
            Repository<Company> tested = null;
            try
            {
                testContext = new StockContext(options);
                tested = new Repository<Company>(testContext);
                tested.Add(company);
                var changesCount = testContext.SaveChanges();

                var actual = tested.Entities.Count();
                var expected = 1;

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
        [ClassData(typeof(_11BitMock))]
        [ClassData(typeof(CdProjectMock))]
        [ClassData(typeof(MbankMock))]
        public void AddStockToLocalDb(Company company)
        {
            string connectionStr = $"server=(localdb)\\MSSQLLocalDB;Initial Catalog={nameof(AddStockToLocalDb)};Integrated Security=True;";
            var options = new DbContextOptionsBuilder<StockContext>()
                .UseSqlServer(connectionStr)
                .Options;

            StockContext testContext = null;
            Repository<Company> tested = null;
            try
            {
                testContext = new StockContext(options);
                testContext.Database.EnsureCreated();
                tested = new Repository<Company>(testContext);
                tested.Add(company);
                var changesCount = testContext.SaveChanges();

                var actual = tested.Entities.Count();
                var expected = 1;

                Assert.Equal(expected, actual);
            }
            finally
            {
                testContext?.Database.EnsureDeleted();
                tested?.Dispose();
                testContext?.Dispose();
            }



            //using (var unitOfWork = new StockEfUnitOfWork(new StockEfTestContext(connectionStr)))
            //{
            //    unitOfWork.Stocks.Repository.Add(company);
            //    unitOfWork.Complete();
            //}

            //using (var connection = new SqlConnection(connectionStr))
            //using (var command = new SqlCommand("Select count(*) from [Companies]", connection))
            //{
            //    connection.Open();
            //    var result = (int)command.ExecuteScalar();
            //    Assert.Equal(1, result);
            //}
        }
        [Theory]
        [ClassData(typeof(MbankMock))]
        public void RemoveSpecificStock(Company company)
        {

            //string connectionStr = $"server=(localdb)\\MSSQLLocalDB;Initial Catalog={nameof(RemoveSpecificStock)};Integrated Security=True;";

            //using (var unitOfWork = new StockEfUnitOfWork(new StockEfTestContext(connectionStr)))
            //{
            //    Assert.Equal(0, unitOfWork.Stocks.Repository.Count());

            //    new CompanyBulkInserter(connectionStr).BulkInsert(company);

            //    Assert.Equal(1, unitOfWork.Stocks.Repository.Count());

            //    unitOfWork.Stocks.Repository.Remove(company);
            //    unitOfWork.Complete();

            //    Assert.Equal(0, unitOfWork.Stocks.Repository.Count());
            //}

            //using (var connection = new SqlConnection(connectionStr))
            //using (var command = new SqlCommand("Select count(*) from [Companies]", connection))
            //{
            //    connection.Open();
            //    var result = (int)command.ExecuteScalar();
            //    Assert.Equal(0, result);
            //}
        }
    }
}
