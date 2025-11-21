using ECommerceWeb.MVC.Models.AuthViewModels;
using ECommerceWeb.MVC.Models.OrderviewModels;
using Microsoft.AspNetCore.Mvc;
using System.Text.Json;

namespace ECommerceWeb.MVC.Controllers
{
    public class AuthController : Controller
    {
        private const string SessionUserKey = "UserInfo";

        // Kayıt olan kullanıcılar burada tutulacak (demo)
        public static List<UserInformation> Users = new List<UserInformation>();

        private bool CheckLogin() => HttpContext.Session.GetString(SessionUserKey) != null;

        // GET: Login
        [Route("account/login")]
        public IActionResult Login()
        {
            if (CheckLogin())
                return RedirectToAction("Details", "Profile");

            return View();
        }

        // POST: Login
        [HttpPost]
        [Route("account/login")]
        public IActionResult Login(LoginViewModel model)
        {
            if (!ModelState.IsValid)
                return View(model);

            //  1) Önce kayıtlı kullanıcılar listesinde ara
            var existingUser = AuthController.Users
                .FirstOrDefault(u => u.Email == model.Email && u.Password == model.Password);

            if (existingUser != null)
            {
                // kullanıcı bulundu → session'a yaz
                HttpContext.Session.SetString(SessionUserKey, JsonSerializer.Serialize(existingUser));
                return RedirectToAction("Details", "Profile");
            }

            //  2) Demo user login
            if (model.Email == "test@test.com" && model.Password == "1234")
            {
                var demoUser = new UserInformation
                {
                    FullName = "Test User",
                    Email = model.Email,
                    Password = model.Password
                };

                HttpContext.Session.SetString(SessionUserKey, JsonSerializer.Serialize(demoUser));
                return RedirectToAction("Details", "Profile");
            }

            // 3) Hatalı giriş
            ModelState.AddModelError("", "Incorrect email or password");
            return View(model);
        }

        // GET: Register
        [Route("account/register")]
        public IActionResult Register()
        {
            if (CheckLogin())
                return RedirectToAction("Details", "Profile");

            return View();
        }

        // POST: Register
        [HttpPost]
        [Route("account/register")]
        public IActionResult Register(RegisterViewModel model)
        {
            if (!ModelState.IsValid)
                return View(model);

            // Yeni kullanıcı oluştur
            var user = new UserInformation
            {
                FullName = model.Username,
                Email = model.Email,
                Password = model.Password,
                Phone = "555-123-4567",
                Address = "Demo Address",
                City = "Demo City",
                ZipCode = "00000"
            };

            // KULLANICIYI LİSTEYE EKLE
            Users.Add(user);

            // Giriş yapmış gibi session’a yaz
            HttpContext.Session.SetString(SessionUserKey, JsonSerializer.Serialize(user));

            return RedirectToAction("Details", "Profile");
        }

        // Logout
        [Route("account/logout")]
        public IActionResult LogOut()
        {
            HttpContext.Session.Clear();
            return RedirectToAction("Login");
        }
    }
}