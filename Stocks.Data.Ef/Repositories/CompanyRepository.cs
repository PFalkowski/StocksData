﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore;
using Stocks.Data.Ef.DataTransferObjects;
using Stocks.Data.Model;

namespace Stocks.Data.Ef.Repositories
{
    public class CompanyRepository : Repository<Company>, ICompanyRepository
    {
        public CompanyRepository(DbContext context) : base(context)
        {
        }

        public Company GetById(string ticker)
        {
            return Entities
                .Where(x => x.Ticker.Equals(ticker))
                .Include(x => x.Quotes)
                .SingleOrDefault();
        }

        public override IEnumerable<Company> GetAll()
        {
            return Entities
                .Include(x => x.Quotes);
        }

        public override IEnumerable<Company> GetAll(Expression<Func<Company, bool>> predicate)
        {
            return Entities
                .Include(x => x.Quotes)
                .Where(predicate.Compile());
        }

        public List<CompanySummaryDto> Summary()
        {
            return Entities
                .Include(x => x.Quotes)
                .Select(x => new CompanySummaryDto
                {
                    Ticker = x.Ticker,
                    QuotesCount = x.Quotes.Count,
                    FirstQuote = x.Quotes.OrderByDescending(q => q.Date).Last(),
                    LastQuote = x.Quotes.OrderByDescending(q => q.Date).First()
                })
                .ToList();
        }
    }
}
