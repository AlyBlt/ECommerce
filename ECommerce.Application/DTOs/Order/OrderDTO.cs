
namespace ECommerce.Application.DTOs.Order
{
    public class OrderDTO
    {
        public int Id { get; set; }
        public string OrderCode { get; set; } = null!;
        public decimal TotalPrice { get; set; }
        public string PaymentMethod { get; set; } = "Credit Card";
        public DateTime CreatedAt { get; set; }

        // Teslimat bilgileri
        public string DeliveryAddress { get; set; } = null!;
        public string DeliveryFullName { get; set; } = null!;
        public string DeliveryPhone { get; set; } = null!;

        // User bilgileri
        public int UserId { get; set; }
        public string UserFullName { get; set; }
        public string UserEmail { get; set; } = "";
        public string UserPhone { get; set; } = "";
        public string UserAddress { get; set; } = "";

        // Order Items
        public List<OrderItemDTO> Items { get; set; } = new List<OrderItemDTO>();
    }
}
