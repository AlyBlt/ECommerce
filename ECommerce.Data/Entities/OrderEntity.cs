using System.ComponentModel.DataAnnotations;

namespace ECommerce.Data.Entities
{
    public class OrderEntity
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        public string OrderCode { get; set; } = null!;
        public string Address { get; set; } = null!;
        public DateTime CreatedAt { get; set; } = DateTime.Now;
        public string PaymentMethod { get; set; } = "Credit Card"; // varsayılan değer

        // Navigation properties
        public UserEntity? User { get; set; }   // Order kimin
        public ICollection<OrderItemEntity> OrderItems { get; set; } = new List<OrderItemEntity>();
    }
}
