using ECommerce.Application.DTOs.Product;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ECommerce.Application.DTOs.Order
{
    public class OrderItemDTO
    {
        public int ProductId { get; set; }
        public string ProductName { get; set; } = null!;
        public decimal UnitPrice { get; set; }
        public byte Quantity { get; set; }
        public string ImageUrl { get; set; } = "/img/product/default.jpg";
        public decimal TotalPrice => UnitPrice * Quantity;
        public List<ProductDTO> Product { get; set; } = new List<ProductDTO>();
    }
}
