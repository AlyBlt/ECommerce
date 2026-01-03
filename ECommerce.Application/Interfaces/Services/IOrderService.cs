using ECommerce.Application.DTOs.Cart;
using ECommerce.Application.DTOs.Order;
using ECommerce.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ECommerce.Application.Interfaces.Services
{
    public interface IOrderService
    {
        Task<OrderDTO> CreateOrderAsync(int userId, string address, string paymentMethod, List<CartItemDTO> cartItems, string deliveryFullName, string deliveryPhone);
        Task<IEnumerable<OrderDTO>> GetOrdersByUserAsync(int userId);
        Task<OrderDTO?> GetOrderAsync(int orderId);
    }
}
