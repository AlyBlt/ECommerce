using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ECommerce.Application.DTOs.Cart
{
    public class CartItemDTO
    {
        public int ProductId { get; set; }
        public string Name { get; set; } = null!;
        public decimal Price { get; set; }
        public byte Quantity { get; set; }
        public string? ImageUrl { get; set; }
        public decimal TotalPrice => Price * Quantity;
    }
}
