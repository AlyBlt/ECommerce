

using ECommerce.Application.DTOs.Cart;

namespace ECommerce.Application.DTOs.Order
{
    public class OrderRequestDTO
    {
        public int UserId { get; set; }
        public string Address { get; set; } = string.Empty;
        public string PaymentMethod { get; set; } = string.Empty;
        public string DeliveryFullName { get; set; } = string.Empty;
        public string DeliveryPhone { get; set; } = string.Empty;
        public List<CartItemDTO> CartItems { get; set; } = new();
    }
}
