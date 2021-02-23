using System;
using System.Collections.Generic;
using StandardInterfaces;
using Stocks.Data.Ef.DataTransferObjects;
using Stocks.Data.Model;

namespace Stocks.Data.Ef
{
    public interface ICompanyRepository : IRepository<Company>, IDisposable
    {
        Company GetById(string ticker);
        List<CompanySummaryDto> Summary();
    }
}