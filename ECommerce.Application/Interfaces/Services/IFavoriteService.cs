using ECommerce.Application.DTOs.Favorite;
using ECommerce.Domain.Entities;
using System.Collections.Generic;

namespace ECommerce.Application.Interfaces.Services
{
    public interface IFavoriteService
    {
        Task<IEnumerable<FavoriteDTO>> GetByUserAsync(int userId, string? token = null); // token eklendi
        Task AddAsync(int userId, int productId);
        Task RemoveAsync(int userId, int productId);
        Task<bool> ExistsAsync(int userId, int productId);
        Task ClearAsync(int userId);

        //yeni
        Task BatchAddAsync(int userId, List<int> productIds, string? token = null); // token eklendi
    }
}