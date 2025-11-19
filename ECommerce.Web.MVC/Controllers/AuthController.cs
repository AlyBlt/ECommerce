using AspNetCoreGeneratedDocument;
using Microsoft.AspNetCore.Mvc;

namespace ECommerceWeb.MVC.Controllers
{
    //Aliye Genel Not: Şu an sadece GET ile sayfayı döndürüyor.
    //Ödevin bu kısmında form gönderimi yapılacaksa POST action eklemek gerekiyor.

    //post action daha açmadım burayı view için yaptım -post action metotu içinde işlem yapmak icin dtoyu lazım 
    //buradaki register, register page'inin viewi için var -register post action için ilerleyen zamanlarda register dtosuna göre hareket etmek lazm -tuncay
    public class AuthController : Controller
    {
        //suanki layout'da loginin route'u bu ona uygun sekilde yaptım
        //diger route'larıda ona entegre ettim. -tuncay
        [Route("account/login")]
        public IActionResult Login()
        {
            return View();
        }

        [Route("account/register")]
        public IActionResult Register()
        {
            return View();
        }

        

        //forgotpassword yerine forgot-password yapıldı--ödevde “arama motoru dostu” URL’ler önerilmiş.
        [Route("account/forgot-password")]
        public IActionResult ForgotPassword()
        {
            return View();
        }

        //Aliye Not:
        //Logout action’ı view döndürüyor ama Ödevde “Kullanıcı Çıkış” için sadece action gerekli, view zorunlu değil.
        //Genellikle kullanıcıyı yönlendirir (RedirectToAction).
        [Route("account/logout")]
        public IActionResult LogOut()
        {
            return RedirectToAction("Index", "Home");
        }
    }
}
