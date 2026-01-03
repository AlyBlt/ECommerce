using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ECommerce.Web.Mvc.Models.Order
{
    public class OrderItemViewModel
    {
        public string ProductName { get; set; } = null!;
        public decimal UnitPrice { get; set; }
        public byte Quantity { get; set; }
        public string ImageUrl { get; set; } = "/img/cart/cart-1.jpg";
        public decimal TotalPrice => UnitPrice * Quantity; // otomatik hesaplanan alan
    }
}
