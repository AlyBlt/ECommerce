using Microsoft.AspNetCore.Mvc;

namespace ECommerceWeb.MVC.Controllers
{
    //Aliye Genel Not: Şu an sadece GET ile sayfayı döndürüyor.
    //Ödevin bu kısmında form gönderimi yapılacaksa POST action eklemek gerekiyor.
    public class AuthController : Controller
    {
        [Route("register")]
        public IActionResult Register()
        {
            return View();
        }

        [Route("login")]
        public IActionResult Login()
        {
            return View();
        }

        //forgotpassword yerine forgot-password yapıldı--ödevde “arama motoru dostu” URL’ler önerilmiş.
        [Route("login/forgot-password")]
        public IActionResult ForgotPassword()
        {
            return View();
        }

        //Aliye Not:
        //Logout action’ı view döndürüyor ama Ödevde “Kullanıcı Çıkış” için sadece action gerekli, view zorunlu değil.
        //Genellikle kullanıcıyı yönlendirir (RedirectToAction).
        [Route("logout")] 
        public IActionResult LogOut()
        {
            return View();
            // Çıkış işlemleri
            //return RedirectToAction("Index", "Home"); ??
        }
    }
}
