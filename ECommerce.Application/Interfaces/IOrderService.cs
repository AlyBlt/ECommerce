using ECommerce.Data.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ECommerce.Application.Interfaces
{
    public interface IOrderService
    {
        OrderEntity CreateOrder(int userId, string address, string paymentMethod, List<CartItemEntity> cartItems);
        IEnumerable<OrderEntity> GetOrdersByUser(int userId);
        OrderEntity? GetOrder(int orderId);
    }
}
