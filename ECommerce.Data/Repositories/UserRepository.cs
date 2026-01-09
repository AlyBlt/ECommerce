using ECommerce.Application.Interfaces.Repositories;
using ECommerce.Data.DbContexts;
using ECommerce.Domain.Entities;
using Microsoft.EntityFrameworkCore;


namespace ECommerce.Data.Repositories
{
    internal class UserRepository : BaseRepository<UserEntity>, IUserRepository
    {
        public UserRepository(ECommerceDbContext context) : base(context)
        {
        }

        // Query ile kullanıcıları filtreleme veya include ekleme ihtiyacı varsa buraya eklenebilir
        public async Task<List<UserEntity>> GetAllWithRolesAsync()
        {
            return await GetQueryable()
                .Include(u => u.Role)
                .ToListAsync();
        }

        // ID ile kullanıcıyı al, opsiyonel olarak Orders ve Products include et
        public async Task<UserEntity?> GetByIdWithIncludesAsync(int id, bool includeOrders = false, bool includeProducts = false)
        {
            IQueryable<UserEntity> query = GetQueryable().Include(u => u.Role);

            if (includeOrders)
            {
                query = query
                    .Include(u => u.Orders)
                        .ThenInclude(o => o.OrderItems)
                        .ThenInclude(oi => oi.Product);
            }

            if (includeProducts)
            {
                query = query
                    .Include(u => u.Products)
                        .ThenInclude(p => p.Category)
                    .Include(u => u.Products)
                        .ThenInclude(p => p.Images)
                    .Include(u => u.Products)
                        .ThenInclude(p => p.Comments);
            }

            return await query.FirstOrDefaultAsync(u => u.Id == id);
        }


        public async Task<UserEntity?> GetByEmailAsync(string email)
        {
            return await GetQueryable()
                .Include(u => u.Role)
                .FirstOrDefaultAsync(u => u.Email == email);
        }

        public async Task<UserEntity?> GetByResetTokenAsync(string token)
        {
            // Veritabanında ResetToken sütunu ile gelen token'ı karşılaştırıyoruz
            return await GetQueryable()
                .FirstOrDefaultAsync(u => u.PasswordResetToken == token);
        }
    }
}
