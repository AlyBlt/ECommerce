

namespace ECommerce.Domain.Entities
{
    public class UserEntity
    {
        public int Id { get; set; }
        public string Email { get; set; } = null!;
        public string FirstName { get; set; } = null!;
        public string LastName { get; set; } = null!;
        public string Password { get; set; } = null!;
        public int RoleId { get; set; }
        public bool Enabled { get; set; } = true;
        public DateTime CreatedAt { get; set; }
        public bool IsSellerApproved { get; set; } = false;
        public bool HasPendingSellerRequest { get; set; } = false; 
        public bool IsRejected { get; set; } = false;
        public string Phone { get; set; } = null!;
        public string Address { get; set; } = null!;

        // Yeni eklenen alanlar
        public string? PasswordResetToken { get; set; }  // Şifre sıfırlama token'ı
        public DateTime? TokenExpiresAt { get; set; }     // Token'ın geçerlilik süresi

        // Navigation
        public RoleEntity? Role { get; set; }
        public ICollection<ProductEntity> Products { get; set; } = new List<ProductEntity>();
        public ICollection<OrderEntity> Orders { get; set; } = new List<OrderEntity>();
        public ICollection<ProductCommentEntity>? Comments { get; set; } = new List<ProductCommentEntity>();
        public ICollection<CartItemEntity>? CartItems { get; set; } = new List<CartItemEntity>();
    }
}
