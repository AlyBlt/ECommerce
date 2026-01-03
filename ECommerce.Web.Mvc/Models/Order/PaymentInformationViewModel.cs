using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ECommerce.Web.Mvc.Models.Order
{
    public class PaymentInformationViewModel
    {
        [Required]
        public string PaymentMethod { get; set; } = "Credit Card"; // default
        [Required]
        [StringLength(500, ErrorMessage = "Address is too long.")]
        public string DeliveryAddress { get; set; } = null!; // Teslimat adresi
       
        public string DeliveryFullName { get; set; } = null!;  // Teslim alacak kişinin adı

        public string DeliveryPhone { get; set; } = null!;    // Teslim alacak kişinin telefonu
    }
}
