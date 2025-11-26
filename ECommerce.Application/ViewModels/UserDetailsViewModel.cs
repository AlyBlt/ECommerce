using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ECommerce.Application.ViewModels
{
    public class UserDetailsViewModel
    {
        public string FirstName { get; set; } = "";
        public string LastName { get; set; } = "";
        public string Email { get; set; } = "";
        public bool IsSellerApproved { get; set; }
        // Yeni eklenen alanlar
        public string Phone { get; set; } = "";
        public string Address { get; set; } = "";
       
        // FullName sadece okuma için
        public string FullName => $"{FirstName} {LastName}";
    }
}
