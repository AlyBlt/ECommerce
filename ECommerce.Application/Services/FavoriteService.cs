using ECommerce.Application.DTOs.Favorite;
using ECommerce.Application.Interfaces.Repositories;
using ECommerce.Application.Interfaces.Services;
using ECommerce.Domain.Entities;


namespace ECommerce.Application.Services
{
    public class FavoriteService : IFavoriteService
    {
        private readonly IFavoriteRepository _favoriteRepository;

        public FavoriteService(IFavoriteRepository favoriteRepository)
        {
            _favoriteRepository = favoriteRepository;
        }

        public async Task<IEnumerable<FavoriteDTO>> GetByUserAsync(int userId, string? token = null)
        {
            var favorites = await _favoriteRepository.GetByUserWithProductAsync(userId);
            return favorites.Select(f => new FavoriteDTO
            {
                Id = f.Id,
                UserId = f.UserId,
                ProductId = f.ProductId,
                ProductName = f.Product?.Name ?? "Unknown Product",
                ProductPrice = f.Product?.Price ?? 0,
                ProductImageUrl = f.Product?.Images.FirstOrDefault(i => i.IsMain)?.Url ?? "default.jpg",
                CreatedAt = f.CreatedAt
            }).ToList();

        }

        public async Task AddAsync(int userId, int productId)
        {
            if (!await ExistsAsync(userId, productId))
            {
              await _favoriteRepository.AddAsync(
                    new FavoriteEntity 
                    { 
                        UserId = userId, 
                        ProductId = productId 
                    });
                await _favoriteRepository.SaveAsync();
            }
        }

        public async Task RemoveAsync(int userId, int productId)
        {
            var favorite = await _favoriteRepository.GetAsync(userId, productId);
            if (favorite != null)
            {
                await _favoriteRepository.DeleteAsync(favorite);
                await _favoriteRepository.SaveAsync();
            }
        }

        public async Task<bool> ExistsAsync(int userId, int productId)
        {
            return await _favoriteRepository.ExistsAsync(userId, productId);
        }

        public async Task ClearAsync(int userId)
        {
            var favorites = await _favoriteRepository.GetByUserAsync(userId);

            if (favorites.Any())
            {
                foreach (var favorite in favorites)
                {
                    await _favoriteRepository.DeleteAsync(favorite);
                }

                await _favoriteRepository.SaveAsync();
            }
        }

        public async Task BatchAddAsync(int userId, List<int> productIds, string? token = null)
        {
            foreach (var productId in productIds)
            {
                await AddAsync(userId, productId);
            }
            await _favoriteRepository.SaveAsync();
        }
    }
}
