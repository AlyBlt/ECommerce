using ECommerce.Application.Interfaces.Repositories;
using ECommerce.Data.DbContexts;
using ECommerce.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ECommerce.Data.Repositories
{
    internal class FavoriteRepository: BaseRepository<FavoriteEntity>, IFavoriteRepository
    {
        public FavoriteRepository(ECommerceDbContext context): base(context){}

        public async Task<List<FavoriteEntity>> GetByUserWithProductAsync(int userId)
        {
            return await Context.Favorites
                .Include(f => f.Product)
                    .ThenInclude(p => p.Images)
                .Where(f => f.UserId == userId)
                .ToListAsync();
        }

        public async Task<bool> ExistsAsync(int userId, int productId)
        {
            return await Context.Favorites
                .AnyAsync(f => f.UserId == userId && f.ProductId == productId);
        }

        public async Task<FavoriteEntity?> GetAsync(int userId, int productId)
        {
            return await Context.Favorites
                .FirstOrDefaultAsync(f =>
                    f.UserId == userId && f.ProductId == productId);
        }

        public async Task<List<FavoriteEntity>> GetByUserAsync(int userId)
        {
            return await Context.Favorites
                .Where(f => f.UserId == userId)
                .ToListAsync();
        }
    }

}
