using ECommerce.Application.DTOs.Favorite;
using ECommerce.Domain.Entities;
using System.Collections.Generic;

namespace ECommerce.Application.Interfaces.Services
{
    public interface IFavoriteService
    {
        Task<IEnumerable<FavoriteDTO>> GetByUserAsync(int userId);
        Task AddAsync(int userId, int productId);
        Task RemoveAsync(int userId, int productId);
        Task<bool> ExistsAsync(int userId, int productId);
        Task ClearAsync(int userId);
    }
}