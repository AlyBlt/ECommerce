using ECommerce.Application.DTOs.Product;
using ECommerce.Application.DTOs.ProductComment;
using ECommerce.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ECommerce.Application.Interfaces.Services
{
    public interface IProductService
    {
        Task<IEnumerable<ProductDTO>> GetAllAsync();
        Task<ProductDTO?> GetAsync(int id);
        Task AddAsync(ProductDTO dto);
        Task UpdateAsync(ProductDTO dto);
        Task DeleteAsync(int id);
        Task ToggleStatusAsync(int id);
        Task AddCommentAsync(int productId, ProductCommentDTO commentDto);

        //Featured ürünleri alırken sadece Entity döndürüyoruz
        Task<List<ProductDTO>> GetActiveProductsAsync();
        Task<List<ProductDTO>> GetFeaturedProductsAsync();
        Task<List<ProductDTO>> GetDiscountedProductsAsync();
        Task<List<ProductDTO>> GetNewArrivalProductsAsync();
        Task<List<ProductDTO>> GetRelatedProductsAsync(int categoryId, int currentProductId);
        Task<List<ProductDTO>> SearchProductsAsync(string query, string? category, byte? rating);
        Task<bool> HasUserPurchasedProductAsync(int userId, int productId);

    }
}
