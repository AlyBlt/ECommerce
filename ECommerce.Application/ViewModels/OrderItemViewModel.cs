using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ECommerce.Application.ViewModels
{
    public class OrderItemViewModel
    {
        public string ProductName { get; set; } = null!;
        public decimal UnitPrice { get; set; }
        public byte Quantity { get; set; }
        public decimal TotalPrice => UnitPrice * Quantity; // otomatik hesaplanan alan
    }
}
