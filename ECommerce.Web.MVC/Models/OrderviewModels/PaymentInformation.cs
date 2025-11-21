using System.ComponentModel.DataAnnotations;

namespace ECommerceWeb.MVC.Models.OrderviewModels
{
    public class PaymentInformation
    {
        [Required(ErrorMessage = "Payment method is required.")]
        public string PaymentMethod { get; set; }
    }
}