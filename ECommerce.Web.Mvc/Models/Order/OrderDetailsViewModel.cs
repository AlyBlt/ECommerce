using ECommerce.Web.Mvc.Models.User;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ECommerce.Web.Mvc.Models.Order
{
    public class OrderDetailsViewModel
    {
        // Order Summary
        public int Id { get; set; }
        public string OrderCode { get; set; } = "";
        public DateTime CreatedAt { get; set; }
        public decimal TotalPrice { get; set; }
        public string PaymentMethod { get; set; } = "";

        // Teslimat bilgileri (hediye adresi)
        public string DeliveryAddress { get; set; } = null!;
        public string DeliveryFullName { get; set; } = null!; // Teslim alacak kişinin adı
        public string DeliveryPhone { get; set; } = null!; // Teslim alacak kişinin phone

        public string Phone { get; set; }
        public string Email { get; set; } = null!;

        // User Information
        public UserInformationViewModel UserInformation { get; set; } = new();

        // Order Items
        public List<OrderItemViewModel> Items { get; set; } = new();

        public PaymentInformationViewModel PaymentInformation { get; set; } = new();
    }
}
