using ECommerce.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ECommerce.Application.Interfaces.Repositories
{
    public interface IOrderRepository : IBaseRepository<OrderEntity>
    {
        Task<OrderEntity> CreateOrderAsync(OrderEntity order, List<CartItemEntity> cartItems);
        Task<List<OrderEntity>> GetOrdersByUserAsync(int userId);
        Task<OrderEntity?> GetOrderWithDetailsAsync(int orderId);
    }
}
