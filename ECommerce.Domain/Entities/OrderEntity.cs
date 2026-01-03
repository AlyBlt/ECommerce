using System.ComponentModel.DataAnnotations;

namespace ECommerce.Domain.Entities
{
    public class OrderEntity
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        public string OrderCode { get; set; } = null!;

        // Kullanıcının profil adresi yerine, bu sipariş için belirlenen teslimat adresi
        public string DeliveryAddress { get; set; } = null!;
        public string DeliveryFullName { get; set; } = null!; // Teslim alacak kişinin adı
        public string DeliveryPhone { get; set; } = null!; //Teslime edilecek kişi phone

        public decimal TotalPrice { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.Now;
        public string PaymentMethod { get; set; } = "Credit Card"; // varsayılan değer

        // Navigation properties
        public UserEntity? User { get; set; }   // Order kimin
        public ICollection<OrderItemEntity> OrderItems { get; set; } = new List<OrderItemEntity>();
    }
}
