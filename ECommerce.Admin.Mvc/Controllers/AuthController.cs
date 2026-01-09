using ECommerce.Admin.Mvc.Models.Auth;
using ECommerce.Application.Interfaces.Services;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace ECommerce.Admin.Mvc.Controllers
{
    public class AuthController : Controller
    {
        // ÖNEMLİ: IUserService yerine UserApiService kullanıyoruz
        private readonly IUserService _userApiService;

        public AuthController(IUserService userApiService)
        {
            _userApiService = userApiService;
        }

        [AllowAnonymous]
        [HttpGet]
        public IActionResult Login(bool passive = false, string? returnUrl = null)
        {
            if (passive) ViewBag.PassiveMessage = "Your account has been deactivated. Contact support!";
            if (User.Identity?.IsAuthenticated == true) return RedirectToAction("Index", "Home");
            return View();
        }

        [AllowAnonymous]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(LoginViewModel model)
        {
            if (!ModelState.IsValid) return View(model);

            // API'ye sor: Bu kullanıcı var mı? Varsa Token ver.
            var loginResult = await _userApiService.AuthenticateAsync(model.Email, model.Password);

            if (loginResult == null || loginResult.User == null)
            {
                TempData["Error"] = "Invalid email or password.";
                return View(model);
            }

            var userDto = loginResult.User;
          
            // Yetki ve Durum Kontrolleri
            // Eğer rol Admin DEĞİLSE VE SystemAdmin DEĞİLSE içeri alma
            if (userDto.RoleName != "Admin" && userDto.RoleName != "SystemAdmin")
            {
                TempData["Error"] = "You do not have permission to access the Admin Panel.";
                return View(model);
            }

            // Durum Kontrolü
            if (!userDto.Enabled)
            {
                ModelState.AddModelError("", "Your account is disabled. Contact support!");
                return View(model);
            }

            // API'DEN GELEN TOKEN'I COOKIE'YE YAZ (Handler buradan okuyacak)
            CookieOptions option = new CookieOptions
            {
                Expires = DateTime.Now.AddHours(2),
                HttpOnly = true, // Güvenlik: JavaScript erişemez
                Secure = false,    // true ise sadece HTTPS üzerinden gönderilir-ama şimdilik deneme kalsın
                SameSite = SameSiteMode.Lax,
                Path = "/"
            };
            // ÖNEMLİ: İsmi JwtHeaderHandler içindekiyle aynı yapıyoruz
            Response.Cookies.Append("Admin_JwtToken", loginResult.Token, option);
            HttpContext.Items["jwtToken"] = loginResult.Token;

            // Claim Oluşturma ve Cookie Sign-In
            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, userDto.Id.ToString()),
                new Claim(ClaimTypes.Name, $"{userDto.FirstName} {userDto.LastName}"),
                new Claim(ClaimTypes.Role, userDto.RoleName),
                new Claim(ClaimTypes.Email, userDto.Email),
                new Claim("Enabled", loginResult.User.Enabled.ToString())
            };

            // ÖDEV MADDESİ: API'den gelen Token'ı Cookie'de sakla 
            // Bu sayede her istekte Token'ı alıp Header'a ekleyebileceğiz.
            // Not: UserApiService içinde token'ı bir yere (örn: HttpContext) kaydettiğinden emin olmalıyız.

            var identity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
            var principal = new ClaimsPrincipal(identity);

            await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, principal);

            return RedirectToAction("Index", "Home");
        }

        [Authorize]
        public async Task<IActionResult> Logout()
        {
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            // İsim aynı olmalı:
            Response.Cookies.Delete("Admin_JwtToken");
            return RedirectToAction("Login");
        }

        [AllowAnonymous]
        [HttpGet]
        public IActionResult ForgotPassword() => View();

        [HttpPost]
        public async Task<IActionResult> ForgotPassword(string email)
        {
            var result = await _userApiService.InitiatePasswordResetAsync(email);
            if (result)
            {
                ViewBag.Message = "If an account exists with this email, a reset link has been sent.";
                return View();
            }
            ModelState.AddModelError("", "Something went wrong.");
            return View();
        }

        [AllowAnonymous]
        [HttpGet]
        public IActionResult ResetPassword(string token)
        {
            // Token'ı sayfadaki hidden field'a taşımak için model oluşturduk
            return View(new ResetPasswordViewModel { Token = token });
        }

        [AllowAnonymous]
        [HttpPost]
        public async Task<IActionResult> ResetPassword(ResetPasswordViewModel model)
        {
            if (!ModelState.IsValid) return View(model);

            var result = await _userApiService.ResetPasswordAsync(model.Token, model.Password);
            if (result)
            {
                TempData["Success"] = "Your password has been reset. You can now login.";
                return RedirectToAction("Login");
            }

            ModelState.AddModelError("", "Invalid or expired token.");
            return View(model);
        }
    }
}