using ECommerce.Application.Interfaces.Repositories;
using ECommerce.Application.Interfaces.Services;
using ECommerce.Domain.Entities;
using System.Collections.Generic;
using System.Linq;
using ECommerce.Application.DTOs.Cart;

namespace ECommerce.Application.Services
{
    public class CartService : ICartService
    {
        private readonly ICartItemRepository _cartItemRepository;


        public CartService(ICartItemRepository cartItemRepository)
        {
            _cartItemRepository = cartItemRepository;
        }


        public async Task<IEnumerable<CartItemDTO>> GetCartItemsAsync(int userId)
        {
            var entities = await _cartItemRepository.GetCartItemsWithProductAndImagesAsync(userId);
           
            // MANUEL MAPPING
            return entities.Select(item => new CartItemDTO
            {
                ProductId = item.ProductId,
                Name = item.Product?.Name ?? "Unknown Product",
                Price = item.Product?.Price ?? 0,
                Quantity = item.Quantity,
                // Eğer CartItem'da özel bir resim yoksa ürünün ana resmini al
                ImageUrl = item.ImageUrl ?? item.Product?.Images?.FirstOrDefault(i => i.IsMain)?.Url
            });
        }

        public async Task AddToCartAsync(int userId, CartAddDTO dto)
        {
            // Repository Include ile ürünü de getiriyor, mapping için lazım
            var item = await _cartItemRepository.GetCartItemAsync(userId, dto.ProductId);

            if (item != null)
            {
                // Mevcut ürün varsa miktar artır
                item.Quantity += dto.Quantity;
                await _cartItemRepository.UpdateAsync(item);
            }
            else
            {
                // product bilgisi repository include ile geliyor
                var product = await _cartItemRepository.GetProductForCartAsync(dto.ProductId);

                if (product == null || !product.Enabled)
                    throw new Exception("Product not available");

                await _cartItemRepository.AddAsync(new CartItemEntity
                {
                    UserId = userId,
                    ProductId = product.Id,
                    Quantity = dto.Quantity,
                    CreatedAt = DateTime.Now,
                    ImageUrl = product.Images.FirstOrDefault(i => i.IsMain)?.Url
                });
            }

            await _cartItemRepository.SaveAsync();
        }

        public async Task UpdateCartItemAsync(int userId, int productId, int quantity)
        {
            var item = await _cartItemRepository.GetCartItemAsync(userId, productId);
            if (item != null)
            {
                if (quantity <= 0)
                {
                    await _cartItemRepository.DeleteAsync(item);  
                }
                else
                {
                    item.Quantity = (byte)quantity;
                    await _cartItemRepository.UpdateAsync(item);  
                }

                await _cartItemRepository.SaveAsync();  
            }
        }

        public async Task RemoveCartItemAsync(int userId, int productId)
        {
            var item = await _cartItemRepository.GetCartItemAsync(userId, productId);
            if (item != null)
            {
                await _cartItemRepository.DeleteAsync(item);
                await _cartItemRepository.SaveAsync();
            }
        }

        public async Task ClearCartAsync(int userId)
        {
            var items = await _cartItemRepository.GetCartItemsWithProductAndImagesAsync(userId);
            foreach (var item in items)
            {
               await _cartItemRepository.DeleteAsync(item);
            }
            await _cartItemRepository.SaveAsync();
        }


    }
}