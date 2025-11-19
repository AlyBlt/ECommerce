using Microsoft.AspNetCore.Mvc;

namespace ECommerceWeb.MVC.Controllers
{
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

        [Route("login/forgotpassword")]
        public IActionResult ForgotPassword()
        {
            return View();
        }

        [Route("logout")]
        public IActionResult LogOut()
        {
            return View();
        }
    }
}
