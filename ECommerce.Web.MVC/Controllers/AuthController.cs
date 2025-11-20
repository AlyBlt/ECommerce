using AspNetCoreGeneratedDocument;
using ECommerceWeb.MVC.Models.AuthViewModels;
using Microsoft.AspNetCore.Mvc;

namespace ECommerceWeb.MVC.Controllers
{
    //GENEL NOT: ALİYE -- Veritabanı olmadığı için en azından test edebilmek adına model kullanarak session/cache ile geçici hafızayla yaptım.
    //Modeller daha sonra veritabanına geçince application katmanına aktarılır, daha kolaylık olur.
    //Bu yöntem sadece demo/test amaçlı. Gerçek projelerde veritabanı ve güvenli şifreleme kullanmak şart tabi.
    //Session ile sadece kullanıcı oturumunu saklayabiliriz; kayıt işlemi de şimdilik statik bir kullanıcı üzerinden test edilebilir.
    public class AuthController : Controller
    {
        
        [Route("account/login")]
        public IActionResult Login()
        {
            return View();
        }

        //Session ile test için Login post eklendi
        [HttpPost]
        public IActionResult Login(LoginViewModel model)
        {
            if (ModelState.IsValid)
            {
                // Örnek: statik kullanıcı (username: test@test.com, password: 1234)
                if (model.Email == "test@test.com" && model.Password == "1234")
                {
                    // Session’a kullanıcı bilgilerini kaydet
                    HttpContext.Session.SetString("UserEmail", model.Email);
                    HttpContext.Session.SetString("Username", "Test User");

                    return RedirectToAction("Index", "Home");
                }
                else
                {
                    ModelState.AddModelError("", "Email veya şifre yanlış");
                }
            }
            return View(model);
        }


        [Route("account/register")]
        public IActionResult Register()
        {
            return View();
        }

        [HttpPost]
        public IActionResult Register(RegisterViewModel model)
        {
            if (ModelState.IsValid)
            {
                // Demo amaçlı: session’a kullanıcı ekleme
                HttpContext.Session.SetString("UserEmail", model.Email);
                HttpContext.Session.SetString("Username", model.Username);

                return RedirectToAction("Index", "Home");
            }
            return View(model);
        }



        //forgotpassword yerine forgot-password yapıldı--ödevde “arama motoru dostu” URL’ler önerilmiş.
        [Route("account/forgot-password")]
        public IActionResult ForgotPassword()
        {
            return View();
        }

        [HttpPost]
        public IActionResult ForgotPassword(ForgotPasswordViewModel model)
        {
            if (ModelState.IsValid)
            {
                // Demo amaçlı: sadece session veya log
                ViewBag.Message = $"Şifre sıfırlama talimatı {model.Email} adresine gönderildi.";
            }
            return View(model);
        }

        
       
        //Genellikle kullanıcıyı yönlendirir (RedirectToAction).
        [Route("account/logout")]
        public IActionResult LogOut()
        {
            //return RedirectToAction("Index", "Home");
           
            // Session temizle
            HttpContext.Session.Clear();
            return RedirectToAction("Login");
            
        }

        public IActionResult Details()
        {
            var email = HttpContext.Session.GetString("UserEmail");
            var username = HttpContext.Session.GetString("Username");

            if (string.IsNullOrEmpty(email))
            {
                return RedirectToAction("Login", "Auth");
            }

            ViewBag.Email = email;
            ViewBag.Username = username;

            return View();
        }



    }
}
