using ECommerce.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ECommerce.Application.Interfaces.Repositories
{
    public interface ICartItemRepository : IBaseRepository<CartItemEntity>
    {
        Task<List<CartItemEntity>> GetCartItemsWithProductAndImagesAsync(int userId);
        Task<CartItemEntity?> GetCartItemAsync(int userId, int productId);
        Task<ProductEntity?> GetProductForCartAsync(int productId);
    }

}