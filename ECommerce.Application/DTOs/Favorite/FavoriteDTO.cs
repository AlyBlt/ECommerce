

namespace ECommerce.Application.DTOs.Favorite
{
    public class FavoriteDTO
    {
        public int Id { get; set; }
        public int UserId { get; set; }         // Hangi kullanıcıya ait
        public int ProductId { get; set; }      // Hangi ürün favori
        public string ProductName { get; set; } = null!;
        public decimal ProductPrice { get; set; }
        public string ProductImageUrl { get; set; } = null!;
        public DateTime CreatedAt { get; set; }
    }
}
