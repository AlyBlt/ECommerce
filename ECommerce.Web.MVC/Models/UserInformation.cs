using System.ComponentModel.DataAnnotations;

namespace ECommerceWeb.MVC.Models
{
    public class UserInformation
    {
        [Required(ErrorMessage = "Ad Soyad alanı zorunludur.")]
        public string FullName { get; set; }

        [Required(ErrorMessage = "Adres alanı zorunludur.")]
        public string Address { get; set; }

        [Required(ErrorMessage = "Şehir alanı zorunludur.")]
        public string City { get; set; }

        [Required(ErrorMessage = "Posta Kodu alanı zorunludur.")]
        public string ZipCode { get; set; }

        [Required(ErrorMessage = "Telefon alanı zorunludur.")]
        [Phone(ErrorMessage = "Geçerli bir telefon numarası girin.")]
        public string Phone { get; set; }

        [Required(ErrorMessage = "Email alanı zorunludur.")]
        [EmailAddress(ErrorMessage = "Geçerli bir email adresi girin.")]
        public string Email { get; set; }
    }
}
