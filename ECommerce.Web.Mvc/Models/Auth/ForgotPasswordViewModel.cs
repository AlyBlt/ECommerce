using System.ComponentModel.DataAnnotations;

namespace ECommerce.Web.Mvc.Models.Auth
{
    public class ForgotPasswordViewModel
    {
        [Required(ErrorMessage = "Email is required")]
        [EmailAddress(ErrorMessage = "Invalid Email Address")]
        public string Email { get; set; }
    }
}
