﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DotnetCoding.Core.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace DotnetCoding.Infrastructure.Repositories
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly DbContextClass _dbContext;
        public IProductRepository Products { get; }

        public UnitOfWork(DbContextClass dbContext,
                            IProductRepository productRepository)
        {
            _dbContext = dbContext;
            Products = productRepository;
        }

        //public int Save()
        //{
        //    return _dbContext.SaveChanges();
        //}

        public async Task<int> Save()
        {
            return await _dbContext.SaveChangesAsync();  // Use SaveChangesAsync for async saving
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (disposing)
            {
                _dbContext.Dispose();
            }
        }

    }
}
