using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ECommerce.Web.Mvc.Models.Cart
{
    public class CartItemViewModel
    {
        public int ProductId { get; set; }
        public string Name { get; set; } = null!;
        public decimal Price { get; set; }
        public byte Quantity { get; set; }
        public decimal TotalPrice => Price * Quantity;
        public string? ImageUrl { get; set; }
    }
}
