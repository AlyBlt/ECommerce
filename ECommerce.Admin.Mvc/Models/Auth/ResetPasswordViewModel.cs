using System.ComponentModel.DataAnnotations;

namespace ECommerce.Admin.Mvc.Models.Auth
{
    public class ResetPasswordViewModel
    {
        [Required]
        public string Token { get; set; } // Maildeki linkten gelecek gizli anahtar

        [Required(ErrorMessage = "New password is required")]
        [DataType(DataType.Password)]
        public string Password { get; set; }

        [Required(ErrorMessage = "Please confirm your password")]
        [DataType(DataType.Password)]
        [Compare("Password", ErrorMessage = "Passwords do not match")]
        public string ConfirmPassword { get; set; }
    }
}
