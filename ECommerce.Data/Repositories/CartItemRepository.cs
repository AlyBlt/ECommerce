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
    internal class CartItemRepository : BaseRepository<CartItemEntity>, ICartItemRepository
    {
        public CartItemRepository(ECommerceDbContext context)
            : base(context)
        {
        }

        public async Task<List<CartItemEntity>> GetCartItemsWithProductAndImagesAsync(int userId)
        {
            return await Context.CartItems
                .Where(c => c.UserId == userId)
                .Include(c => c.Product)
                .ThenInclude(p => p.Images)
                .ToListAsync();
        }

        public async Task<CartItemEntity?> GetCartItemAsync(int userId, int productId)
        {
            return await Context.CartItems
                .Include(c => c.Product)
                .ThenInclude(p => p.Images)
                .FirstOrDefaultAsync(c => c.UserId == userId && c.ProductId == productId);
        }

        public async Task<ProductEntity?> GetProductForCartAsync(int productId)
        {
            return await Context.Products
                .Include(p => p.Images)
                .FirstOrDefaultAsync(p => p.Id == productId && p.Enabled);
        }

        // Diğer metotları BaseRepository’den kullanıyoruz (Add, Update, Delete, SaveAsync vb.)
    }
}
