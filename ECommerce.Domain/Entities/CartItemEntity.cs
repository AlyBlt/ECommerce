using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ECommerce.Domain.Entities
{
    public class CartItemEntity
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        public int ProductId { get; set; }
        public byte Quantity { get; set; }
        public DateTime CreatedAt { get; set; }
        public string? ImageUrl { get; set; }

       
        // Navigation
        public UserEntity User { get; set; } = null!;
        public ProductEntity Product { get; set; } = null!;
    }
}
