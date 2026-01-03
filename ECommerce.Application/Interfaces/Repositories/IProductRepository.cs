using ECommerce.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ECommerce.Application.Interfaces.Repositories
{
    public interface IProductRepository : IBaseRepository<ProductEntity>
    {
        Task<List<ProductEntity>> GetAllWithIncludesAsync();
        Task<ProductEntity?> GetByIdWithIncludesAsync(int id);
        Task<List<ProductEntity>> GetActiveProductsAsync();
        Task<List<ProductEntity>> GetFeaturedProductsAsync();
        Task<List<ProductEntity>> GetDiscountedProductsAsync();
        Task<List<ProductEntity>> GetNewArrivalProductsAsync();
        Task<List<ProductEntity>> GetRelatedProductsAsync(int categoryId, int currentProductId);
        Task<List<ProductEntity>> GetProductsForSearchAsync(string query, string? category, byte? rating);
        IQueryable<ProductEntity> GetAllQuery();
        Task<bool> HasUserPurchasedProductAsync(int userId, int productId);
    }
}
