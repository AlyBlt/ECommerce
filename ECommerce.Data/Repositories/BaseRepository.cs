using ECommerce.Application.Interfaces.Repositories;
using ECommerce.Data.DbContexts;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ECommerce.Data.Repositories
{
    internal class BaseRepository<TEntity> : IBaseRepository<TEntity> where TEntity : class
    {
        private readonly ECommerceDbContext _context;
        private readonly DbSet<TEntity> _dbSet;

        public BaseRepository(ECommerceDbContext context)
        {
            _context = context;
            _dbSet = context.Set<TEntity>();
        }

        public async Task<List<TEntity>> GetAllAsync() => await _dbSet.ToListAsync();
        public async Task<TEntity?> GetByIdAsync(int id) => await _dbSet.FindAsync(id);
        public async Task AddAsync(TEntity entity) => await _dbSet.AddAsync(entity);
        public Task UpdateAsync(TEntity entity)
        {
            _dbSet.Update(entity);
            return Task.CompletedTask;
        }

        public Task DeleteAsync(TEntity entity)
        {
            _dbSet.Remove(entity);
            return Task.CompletedTask;
        }
        public async Task SaveAsync() => await _context.SaveChangesAsync();
        public IQueryable<TEntity> GetQueryable() => _dbSet.AsQueryable();
        protected ECommerceDbContext Context => _context;

    }
}
