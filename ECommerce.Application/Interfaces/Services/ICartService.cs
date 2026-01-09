using ECommerce.Application.DTOs.Cart;
using ECommerce.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ECommerce.Application.Interfaces.Services
{
    public interface ICartService
    {
        //Task<IEnumerable<CartItemDTO>> GetCartItemsAsync(int userId);
        Task AddToCartAsync(int userId, CartAddDTO dto);
        Task UpdateCartItemAsync(int userId, int productId, int quantity);
        Task RemoveCartItemAsync(int userId, int productId);
        Task ClearCartAsync(int userId);
        // Yeni eklenen metod:
        //Task SyncCartAsync(int userId, List<CartAddDTO> items);

        //---------
        Task SyncCartAsync(int userId, List<CartAddDTO> items, string token);
        Task<IEnumerable<CartItemDTO>> GetCartItemsAsync(int userId, string? token = null);
    }
}
