﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using DAL.IRepository;
using DAL.Models;
using Microsoft.EntityFrameworkCore;

namespace DAL.Repository
{
    public class UserRepository<T> : IUserRepository<T> where T : class
    {
        private readonly SWP391_RedRibbonLifeContext _dbContext;
        private DbSet<T> _dbSet;
        public UserRepository(SWP391_RedRibbonLifeContext dbContext)
        {
            _dbContext = dbContext;
            _dbSet = _dbContext.Set<T>();
        }
        public async Task<T> CreateAsync(T dbRecord)
        {
            _dbSet.Add(dbRecord);
            await _dbContext.SaveChangesAsync();
            return dbRecord;
        }

        public async Task<bool> DeleteAsync(T dbRecord)
        {
            _dbSet.Remove(dbRecord);
            await _dbContext.SaveChangesAsync();
            return true;
        }

        public async Task<List<T>> GetAllAsync()
        {
            return await _dbSet.ToListAsync();
        }

        public async Task<List<T>> GetAllWithRelationsAsync(Func<IQueryable<T>, IQueryable<T>> includeFunc = null)
        {
            IQueryable<T> query = _dbSet;
            if (includeFunc != null)
            {
                query = includeFunc(query);
            }
            return await query.ToListAsync();
        }

        public async Task<T?> GetWithRelationsAsync(Expression<Func<T, bool>> filter, bool useNoTracking = false, Func<IQueryable<T>, IQueryable<T>> includeFunc = null)
        {
            IQueryable<T> query = _dbSet;
            if (useNoTracking)
            {
                query = query.AsNoTracking();
            }
            if (includeFunc != null)
            {
                query = includeFunc(query);
            }
            return await query.Where(filter).FirstOrDefaultAsync();
        }

        public async Task<T> GetAsync(Expression<Func<T, bool>> filter, bool useNoTracking = false)
        {
            if (useNoTracking)
            {
                return await _dbSet.AsNoTracking().Where(filter).FirstOrDefaultAsync();
            }
            else
            {
                return await _dbSet.Where(filter).FirstOrDefaultAsync();
            }
        }
        public async Task<List<T>> GetAllByFilterAsync(Expression<Func<T, bool>> filter, bool useNoTracking = false)
        {
            if (useNoTracking)
            {
                return await _dbSet.AsNoTracking().Where(filter).ToListAsync();
            }
            else
            {
                return await _dbSet.Where(filter).ToListAsync();
            }
        }

        //public async Task<T> GetByNameAsync(Expression<Func<T, bool>> filter)
        //{
        //    return await _dbSet.Where(filter).FirstOrDefaultAsync();
        //}

        public async Task<T> UpdateAsync(T dbRecord)
        {
            _dbContext.Update(dbRecord);
            await _dbContext.SaveChangesAsync();
            return dbRecord;
        }

        public async Task<List<T>> GetAllWithRelationsByFilterAsync(Expression<Func<T, bool>> filter, bool useNoTracking = false, Func<IQueryable<T>, IQueryable<T>> includeFunc = null)
        {
            IQueryable<T> query = _dbSet;
            if (useNoTracking)
            {
                query = query.AsNoTracking();
            }
            if (includeFunc != null)
            {
                query = includeFunc(query);
            }
            return await query.Where(filter).ToListAsync();
        }

        public async Task<int> CountAsync(Expression<Func<T, bool>> filter = null)
        {
            if (filter != null)
            {
                return await _dbSet.Where(filter).CountAsync();
            }
            return await _dbSet.CountAsync();
        }

        public async Task<bool> AnyAsync(Expression<Func<T, bool>> filter)
        {
            return await _dbSet.AnyAsync(filter);
        }
    }
}
