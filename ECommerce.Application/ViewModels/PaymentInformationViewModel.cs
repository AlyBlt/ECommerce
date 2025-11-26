using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ECommerce.Application.ViewModels
{
    public class PaymentInformationViewModel
    {
        public string PaymentMethod { get; set; } = "Credit Card"; // default
        public string Address { get; set; } = null!;
    }
}
