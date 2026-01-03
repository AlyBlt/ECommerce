using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ECommerce.Domain.Entities
{
    public class FavoriteEntity
    {
        public int Id { get; set; }
        public int UserId { get; set; }          // Hangi kullanıcıya ait
        public int ProductId { get; set; }       // Hangi ürün favori
        public DateTime CreatedAt { get; set; } = DateTime.Now;

        // Navigation
        public UserEntity? User { get; set; }
        public ProductEntity? Product { get; set; }
    }
}
