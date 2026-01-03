using ECommerce.Application.DTOs.Order;
using ECommerce.Application.DTOs.Product;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ECommerce.Application.DTOs.User
{
    public class UserDTO
    {
        public int Id { get; set; }
        public string Email { get; set; } = null!;
        public string FirstName { get; set; } = null!;
        public string LastName { get; set; } = null!;
        public string FullName => $"{FirstName} {LastName}";
        public string Password { get; set; } = null!; // Sadece create/update aşamasında lazım olabilir
        public string Phone { get; set; } = null!;
        public string Address { get; set; } = null!;

        // Durum Bilgileri
        public bool Enabled { get; set; }
        public bool IsSellerApproved { get; set; }
        public bool HasPendingSellerRequest { get; set; }
        public bool IsRejected { get; set; }

        // Rol Bilgileri
        public int RoleId { get; set; }
        public string? RoleName { get; set; } // Entity'den map'lenecek

        // Zaman Bilgisi
        public DateTime CreatedAt { get; set; }

        // Güvenlik (Reset Password için)
        public string? PasswordResetToken { get; set; }
        public DateTime? TokenExpiresAt { get; set; }

        public List<OrderDTO>? Orders { get; set; } = new();
        public List<ProductDTO>? Products { get; set; } = new();
    }
}
