﻿using Stocks.Data.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;

namespace Stocks.Data.Ef
{
    public class StockQuotesRepositoryCacheDecorator : IStockQuoteRepository
    {
        private readonly IStockQuoteRepository _stockQuoteRepository;

        public StockQuotesRepositoryCacheDecorator(IStockQuoteRepository stockQuoteRepository)
        {
            _stockQuoteRepository = stockQuoteRepository;
        }

        private List<StockQuote> _cache = null;
        private void RebuildCache()
        {
            _cache = _stockQuoteRepository.GetAll().ToList();
        }

        private void EnsureCacheExists()
        {
            
            if (_cache == null) 
                RebuildCache();
        }

        public int Count()
        {
            EnsureCacheExists();
            return _cache.Count;
        }

        public int Count(Expression<Func<StockQuote, bool>> predicate)
        {
            EnsureCacheExists();
            return _cache.Count(predicate.Compile());
        }
        public StockQuote Get(Expression<Func<StockQuote, bool>> predicate)
        {
            EnsureCacheExists();
            return _cache.FirstOrDefault(predicate.Compile());
        }

        public IEnumerable<StockQuote> GetAll()
        {
            EnsureCacheExists();
            return _cache;
        }

        public IEnumerable<StockQuote> GetAll(Expression<Func<StockQuote, bool>> predicate)
        {
            EnsureCacheExists();
            return _cache.Where(predicate.Compile());
        }

        public List<StockQuote> GetAllQuotesFromPreviousNDays(DateTime currentDate, int n)
        {
            EnsureCacheExists();
            var lastDates = GetNTradingDatesBefore(currentDate, n);
            return _cache.Where(x => x.DateParsed >= lastDates.Min() && x.DateParsed <= lastDates.Max())
                .ToList();
        }

        public List<StockQuote> GetAllQuotesFromPreviousSession(DateTime currentDate)
        {
            EnsureCacheExists();
            var lastSessionDates = GetNTradingDatesBefore(currentDate, 1);
            return _cache.Where(x => x.DateParsed.Equals(lastSessionDates.Single()))
                .ToList();
        }

        public List<DateTime> GetNTradingDatesBefore(DateTime currentDate, int n)
        {
            EnsureCacheExists();
            return _cache.Where(x => x.DateParsed < currentDate)
                .Select(x => x.DateParsed)
                .Distinct()
                .OrderByDescending(x => x)
                .Take(n)
                .ToList();
        }

        public List<DateTime> GetTradingDates(DateTime fromInclusive, DateTime toInclusive)
        {
            EnsureCacheExists();
            return _cache.Where(x => x.DateParsed >= fromInclusive && x.DateParsed <= toInclusive)
                .Select(x => x.DateParsed)
                .Distinct()
                .OrderBy(x => x)
                .ToList();
        }

        public void Add(StockQuote entity)
        {
            _stockQuoteRepository.Add(entity);
            RebuildCache();
        }

        public void AddOrUpdate(StockQuote entity)
        {
            _stockQuoteRepository.AddOrUpdate(entity);
            RebuildCache();
        }

        public void AddRange(IEnumerable<StockQuote> entities)
        {
            _stockQuoteRepository.AddRange(entities);
            RebuildCache();
        }

        public void Remove(StockQuote entity)
        {
            _stockQuoteRepository.Remove(entity);
            RebuildCache();
        }

        public void RemoveAll(Expression<Func<StockQuote, bool>> predicate)
        {
            _stockQuoteRepository.RemoveAll(predicate);
            RebuildCache();
        }

        public void RemoveAll()
        {
            _stockQuoteRepository.RemoveAll();
            RebuildCache();
        }

        public void RemoveRange(IEnumerable<StockQuote> entities)
        {
            _stockQuoteRepository.RemoveRange(entities);
            RebuildCache();
        }

        public void Dispose()
        {
            _stockQuoteRepository?.Dispose();
        }
    }
}