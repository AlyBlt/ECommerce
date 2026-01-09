using ECommerce.Application.Interfaces.Repositories;
using ECommerce.Application.Interfaces.Services;
using ECommerce.Domain.Entities;
using ECommerce.Application.DTOs.Order;
using ECommerce.Application.DTOs.Cart;

namespace ECommerce.Application.Services
{
    public class OrderService : IOrderService
    {
        private readonly IOrderRepository _orderRepository;

        public OrderService(IOrderRepository orderRepository)
        {
            _orderRepository = orderRepository;
        }


        public async Task<OrderDTO> CreateOrderAsync(int userId, string address, string paymentMethod, List<CartItemDTO> cartItems, string deliveryFullName, string deliveryPhone)
        {
            var orderEntity = new OrderEntity
            {
                UserId = userId,
                DeliveryAddress = address,
                DeliveryFullName = deliveryFullName,
                DeliveryPhone = deliveryPhone,
                OrderCode = $"ORD-{Guid.NewGuid().ToString().Substring(0, 8).ToUpper()}",
                PaymentMethod = paymentMethod,
                CreatedAt = DateTime.Now,
                TotalPrice = 0
            };

            // Repository hala Entity beklediği için burada dönüşüm yapıyoruz
            var orderItemsForRepo = cartItems.Select(i => new CartItemEntity
            {
                ProductId = i.ProductId,
                Quantity = i.Quantity
            }).ToList();

            var order= await _orderRepository.CreateOrderAsync(orderEntity, orderItemsForRepo);
            return MapToDto(order);
        }

        public async Task<IEnumerable<OrderDTO>> GetOrdersByUserAsync(int userId)
        {
            var orders = await _orderRepository.GetOrdersByUserAsync(userId);
            return orders.Select(MapToDto).ToList();
        }

        public async Task<OrderDTO?> GetOrderAsync(int orderId)
        {
            var order = await _orderRepository.GetOrderWithDetailsAsync(orderId);
            return order == null ? null : MapToDto(order);
        }


        // --- Mapping --- 
        private OrderDTO MapToDto(OrderEntity order)
        {
            return new OrderDTO
            {
                Id = order.Id,
                OrderCode = order.OrderCode,
                TotalPrice = order.TotalPrice,
                PaymentMethod = order.PaymentMethod,
                CreatedAt = order.CreatedAt,
                DeliveryAddress = order.DeliveryAddress,
                DeliveryFullName = order.DeliveryFullName,
                DeliveryPhone = order.DeliveryPhone,
                UserId = order.UserId,
                UserEmail = order.User?.Email ?? "",
                UserPhone = order.User?.Phone ?? "",
                UserFullName = order.User != null ? $"{order.User.FirstName} {order.User.LastName}" : "Unknown User",
                UserAddress = order.User?.Address ?? "",
                Items = order.OrderItems.Select(i => new OrderItemDTO
                {
                    ProductId = i.ProductId,
                    ProductName = i.Product?.Name ?? "Product",
                    UnitPrice = i.UnitPrice,
                    Quantity = i.Quantity,
                    ImageUrl = i.Product?.Images?.FirstOrDefault(img => img.IsMain)?.Url ?? "default.jpg"
                }).ToList()
            };
        }
    }
}
