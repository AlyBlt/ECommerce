using ECommerce.Admin.Mvc.Models.Auth;
using ECommerce.Application.Interfaces.Services;
using ECommerce.Application.Services;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace ECommerce.Admin.Mvc.Controllers
{
    [AllowAnonymous]
    public class AuthController : Controller
    {
        private readonly IUserService _userService;
        
        public AuthController(IUserService userService)
        {
            _userService = userService;
        }

        // Login GET - Kullanıcı Giriş Sayfası
        [HttpGet]
        public IActionResult Login(bool passive = false)
        {
            if (passive)
            {
                ViewBag.PassiveMessage =
                    "Your account has been deactivated by an administrator.";
            }

            if (User.Identity?.IsAuthenticated == true)
            {
                return RedirectToAction("Index", "Home");
            }

            return View();
        }

        // Login POST - Kullanıcı Girişi
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(LoginViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model); // Model geçerli değilse formu tekrar render et
            }

            // Servis artık UserDTO dönüyor
            var userDto = await _userService.AuthenticateAsync(model.Email, model.Password);

            // 1. Kimlik Kontrolü
            if (userDto == null)
            {
                TempData["Error"] = "Invalid email or password.";
                return View(model);
            }

            // 2. Yetki Kontrolü (Sadece Admin girebilir)
            if (userDto.RoleName != "Admin")
            {
                TempData["Error"] = "You do not have permission to access the Admin Panel.";
                return View(model);
            }

            // 3. Durum Kontrolü (Aktif mi?)
            if (!userDto.Enabled)
            {
                ModelState.AddModelError("", "Your account is currently disabled. Please contact support.");
                return View(model);
            }


            var claims = new List<Claim>
             {
             new Claim(ClaimTypes.NameIdentifier, userDto.Id.ToString()),
             new Claim(ClaimTypes.Name, $"{userDto.FirstName} {userDto.LastName}"),
             new Claim(ClaimTypes.Role, userDto.RoleName ?? "Admin"),
              new Claim("Enabled", userDto.Enabled.ToString())
             };

            var identity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);

            var principal = new ClaimsPrincipal(identity);
            await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, principal);

            
            return RedirectToAction("Index", "Home");  // Ana sayfaya yönlendir
        }

        // Logout - Kullanıcı Çıkış Yapma
        public async Task<IActionResult> Logout()
        {
            await HttpContext.SignOutAsync(
                CookieAuthenticationDefaults.AuthenticationScheme);

            return RedirectToAction("Login");
        }
    }
}