using System.ComponentModel.DataAnnotations;

namespace ECommerceWeb.MVC.Models
{
    public class PaymentInformation
    {
        [Required(ErrorMessage = "Ödeme yöntemi seçilmelidir.")]
        public string PaymentMethod { get; set; }
    }
}
