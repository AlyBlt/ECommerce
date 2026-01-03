
using ECommerce.Domain.Entities;

namespace ECommerce.Admin.Mvc.Models.User
{
    public class UserViewModel
    {
        public int Id { get; set; }
        public string FirstName { get; set; } = null!;
        public string LastName { get; set; } = null!;
        public string Email { get; set; } = null!;
        public string RoleName { get; set; } = null!;
        public string Phone { get; set; } = null!;
        public string Address { get; set; } = null!;
        public bool IsSellerApproved { get; set; }
        public bool IsRejected { get; set; } 
        public bool Enabled { get; set; } = true;
        public bool HasPendingSellerRequest { get; set; } = false;

      
    }
}
