using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ECommerce.Application.ViewModels
{
    public class OrderSummaryViewModel
    {
        public int Id { get; set; }
        public string OrderCode { get; set; } = "";
        public DateTime CreatedAt { get; set; }
        public decimal TotalAmount { get; set; }

        public string PaymentMethod { get; set; } = "Credit Card"; // varsayılan değer
        public List<OrderItemViewModel> Items { get; set; } = new List<OrderItemViewModel>();
    }
}
