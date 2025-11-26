using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ECommerce.Application.ViewModels
{
    public class OrderDetailsViewModel
    {
        // Order Summary
        public int Id { get; set; }
        public string OrderCode { get; set; } = "";
        public DateTime CreatedAt { get; set; }
        public decimal TotalPrice { get; set; }
        public string PaymentMethod { get; set; } = "";

        // User Information
        public UserInformationViewModel UserInformation { get; set; } = new();

        // Order Items
        public List<OrderItemViewModel> Items { get; set; } = new();
    }
}
