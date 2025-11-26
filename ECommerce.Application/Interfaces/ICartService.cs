using ECommerce.Data.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ECommerce.Application.Interfaces
{
    public interface ICartService
    {
        IEnumerable<CartItemEntity> GetCartItems(int userId);
        void AddToCart(int userId, int productId, int quantity);
        void UpdateCartItem(int userId, int productId, int quantity);
        void RemoveCartItem(int userId, int productId);
        void ClearCart(int userId);
    }
}
