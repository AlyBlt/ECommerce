using ECommerce.Application.Interfaces;
using ECommerce.Application.Services;
using ECommerce.Application.ViewModels;
using ECommerce.Data.Entities;
using Microsoft.AspNetCore.Mvc;

namespace ECommerce.Web.MVC.Controllers
{
    public class AuthController : Controller
    {
        private readonly IUserService _userService;
        private const string SessionUserId = "UserId";

        public AuthController(IUserService userService)
        {
            _userService = userService;
        }

        private bool IsLoggedIn() => HttpContext.Session.GetInt32(SessionUserId) != null;

        [Route("account/login")]
        public IActionResult Login()
        {
            if (IsLoggedIn()) return RedirectToAction("Details", "Profile");
            return View();
        }

        [HttpPost]
        [Route("account/login")]
        public IActionResult Login(LoginViewModel model)
        {
            if (!ModelState.IsValid)
                return View(model);

            var user = _userService.GetByEmail(model.Email);

            if (user == null || user.Password != model.Password)
            {
                ModelState.AddModelError("", "Incorrect email or password");
                return View(model);
            }

            HttpContext.Session.SetInt32(SessionUserId, user.Id);
            return RedirectToAction("Details", "Profile");
        }

        [Route("account/register")]
        public IActionResult Register()
        {
            if (IsLoggedIn()) return RedirectToAction("Details", "Profile");
            return View();
        }

        [HttpPost]
        [Route("account/register")]
        public IActionResult Register(RegisterViewModel model)
        {
            if (!ModelState.IsValid)
                return View(model);

            // Email tekrar kontrolü
            var existing = _userService.GetByEmail(model.Email);
            if (existing != null)
            {
                ModelState.AddModelError("", "This email is already used.");
                return View(model);
            }

            // Register → DB’ye ekle
            var user = _userService.Register(
                model.Email,
                model.Username.Split(" ")[0],
                model.Username.Contains(" ") ? model.Username.Split(" ")[1] : "",
                model.Password
            );

            // Session’a sadece UserId yazılır
            HttpContext.Session.SetInt32(SessionUserId, user.Id);

            return RedirectToAction("Details", "Profile");
        }

        [Route("account/logout")]
        public IActionResult Logout()
        {
            HttpContext.Session.Clear();
            return RedirectToAction("Login");
        }

        //Forgot password e bak demo bir şey yapılabilir mi

    }
}
