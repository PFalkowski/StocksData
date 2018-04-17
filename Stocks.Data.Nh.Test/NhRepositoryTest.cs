using System.Linq;
using Stocks.Data.Model;
using Stocks.Data.Nh.Test.Mocks;
using Xunit;

namespace Stocks.Data.Nh.Test
{
    public class NhRepositoryTest
    {
        [Theory]
        [ClassData(typeof(_11BitMock))]
        public void AddingStockToNhibernateWorks(Company company)
        {
            var expected = company.Quotes.Count;
            var dbName = nameof(AddingStockToNhibernateWorks);
            UnitTestHelper.RecreateLocalDatabase(dbName);
            string connectionStr = $@"server=(localdb)\MSSQLLocalDB;Initial Catalog={dbName};Integrated Security=True;";

            using (var unitOfWork = new StockNhUnitOfWork(new StockNhContextModelUpdate(connectionStr)))
            {
                unitOfWork.Stocks.Add(company);
                unitOfWork.Complete();

                var all = unitOfWork.Stocks.GetAll();
                Assert.Single(all);
                Assert.Equal(expected, all.First().Quotes.Count);
            }
        }

        [Theory]
        [ClassData(typeof(_11BitMock))]
        public void RemovingSpecificStockFromNhibernateWorks(Company company)
        {
            var expected = company.Quotes.Count;
            var dbName = nameof(AddingStockToNhibernateWorks);
            UnitTestHelper.RecreateLocalDatabase(dbName);
            string connectionStr = $@"server=(localdb)\MSSQLLocalDB;Initial Catalog={dbName};Integrated Security=True;";

            using (var unitOfWork = new StockNhUnitOfWork(new StockNhContextModelUpdate(connectionStr)))
            {
                unitOfWork.Stocks.Add(company);
                unitOfWork.Complete();

                var before = unitOfWork.Stocks.GetAll();
                Assert.Single(before);
                Assert.Equal(expected, before.First().Quotes.Count);

                unitOfWork.Stocks.Remove(company);
                unitOfWork.Complete();

                var after = unitOfWork.Stocks.GetAll();

                Assert.Empty(after);
            }
        }
        
        //[Fact]
        //public void GetSpecificStock()
        //{
        //    const string connectionStr = @"Data Source=(localdb)\MSSQLLocalDB;Initial Catalog=StockMarketDb;Integrated Security=True;Connect Timeout=30;Encrypt=False;TrustServerCertificate=True;ApplicationIntent=ReadWrite;MultiSubnetFailover=False";

        //    using (var unitOfWork = new StockDatabaseUnitOfWork(new StockDbContextModelUpdate(connectionStr)))
        //    {
        //        unitOfWork.StockRepository.GetAll((x) => x.Ticker == "MBANK");
        //        unitOfWork.Complete();
        //    }
        //    //Parallel.ForEach(allFiles, (file) => allStocks.Add(file.Value.DeserializeFromCsv(new StockQuoteCsvClassMap()).ToList()));

        //}
    }
}
