using ECommerce.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ECommerce.Application.Interfaces.Repositories
{
    public interface IFavoriteRepository : IBaseRepository<FavoriteEntity>
    {
        Task<List<FavoriteEntity>> GetByUserWithProductAsync(int userId);
        Task<bool> ExistsAsync(int userId, int productId);
        Task<FavoriteEntity?> GetAsync(int userId, int productId);
        Task<List<FavoriteEntity>> GetByUserAsync(int userId);
    }
}
