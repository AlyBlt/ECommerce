using ECommerce.Application.DTOs.Cart;
using ECommerce.Application.DTOs.User;
using ECommerce.Application.Interfaces.Services;
using ECommerce.Web.Mvc.Helpers;
using ECommerce.Web.Mvc.Models.Auth;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;
using System.Security.Claims;

namespace ECommerce.Web.Mvc.Controllers
{
    public class AuthController : Controller
    {
        private readonly IUserService _userApiService; 
        private readonly ICartService _cartApiService;
        private readonly IFavoriteService _favoriteApiService;
        private readonly IEmailService _emailService;

        public AuthController(
            IUserService userApiService,
            ICartService cartApiService,
            IFavoriteService favoriteApiService,
            IEmailService emailService)
        {
            _userApiService = userApiService;
            _cartApiService = cartApiService;
            _favoriteApiService = favoriteApiService;
            _emailService = emailService;
        }

        private bool IsLoggedIn() => User.Identity?.IsAuthenticated == true;

        // ---------------- MERGE METHODS ----------------

        private async Task MergeSessionCartToDbAsync(int userId, string token)
        {
            var sessionCart = SessionHelper.GetCart(HttpContext);
            if (sessionCart != null && sessionCart.Items.Any())
            {
                try
                {
                    var itemsToSync = sessionCart.Items.Select(i => new CartAddDTO
                    {
                        ProductId = i.ProductId,
                        Quantity = i.Quantity
                    }).ToList();

                    // 1. Token ile Sync et
                    await _cartApiService.SyncCartAsync(userId, itemsToSync, token);

                    SessionHelper.ClearCart(HttpContext);
                }
                catch (Exception ex) { Debug.WriteLine($"Sync Error: {ex.Message}"); }
            }

            // 2. Güncel listeyi çekerken de token kullan ki 401 hatası almayalım
            var dbCartItems = await _cartApiService.GetCartItemsAsync(userId, token);
            HttpContext.Session.SetInt32("CartCount", dbCartItems.Sum(x => x.Quantity));
        }

        private async Task MergeSessionFavoritesToDbAsync(int userId, string token) // token eklendi
        {
            var sessionFavs = SessionHelper.GetFavorites(HttpContext);
            if (sessionFavs != null && sessionFavs.Any())
            {
                try
                {
                    var productIds = sessionFavs.Select(x => x.ProductId).ToList();
                    // Token ile gönderiyoruz
                    await _favoriteApiService.BatchAddAsync(userId, productIds, token);

                    SessionHelper.ClearFavorites(HttpContext);
                }
                catch (Exception ex) { Debug.WriteLine($"Fav Sync Error: {ex.Message}"); }
            }

            // Güncel listeyi çekip session'ı tazele
            try
            {
                var dbFavs = await _favoriteApiService.GetByUserAsync(userId, token);
                HttpContext.Session.SetInt32("FavoritesCount", dbFavs.Count());
            }
            catch { }
        }

        // ---------------- LOGIN ----------------
        [AllowAnonymous]
        [HttpGet("account/login")]
        public IActionResult Login(bool passive = false, string? returnUrl = null)
        {
            // Eğer kullanıcı zaten giriş yapmışsa ve returnUrl login değilse yönlendir
            // Ama returnUrl doluysa döngüyü kırmak için IsLoggedIn kontrolünü atlayabiliriz
            if (IsLoggedIn() && string.IsNullOrEmpty(returnUrl))
                return RedirectToAction("Index", "Home");

            if (passive)
            {
                ViewBag.PassiveMessage = "Your account is disabled. Contact support.";
            }

            ViewBag.ReturnUrl = returnUrl;
            return View();
        }

        [AllowAnonymous]
        [HttpPost("account/login")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(LoginViewModel model, string? returnUrl)
        {
            if (!ModelState.IsValid)
                return View(model);

           var authResult = await _userApiService.AuthenticateAsync(model.Email, model.Password);

            if (authResult == null || authResult.User == null)
            {
                ModelState.AddModelError("", "Invalid email or password.");
                return View(model);
            }

            if (!authResult.User.Enabled)
            {
                ModelState.AddModelError("", "Your account is disabled. Contact support.");
                return View(model);
            }

            // ÖDEV ŞARTI: Admin Web'e giremez --- ama SystemAdmin'in girmesi düşünülebilir sonrasında!!
            var restrictedRoles = new[] { "Admin", "SystemAdmin" };

            if (restrictedRoles.Contains(authResult.User.RoleName, StringComparer.OrdinalIgnoreCase))
            {
                ModelState.AddModelError("", "Admins cannot login to the web store.");
                return View(model);
            }

            // 1. ADIM: JWT Token'ı Cookie'ye bas (Handler'ın okuması için)
            Response.Cookies.Append("Web_JwtToken", authResult.Token, new CookieOptions
            {
                HttpOnly = true,
                Secure = false, //true, // Localhost'ta sorun olursa false deneyebiliriz--Geliştirme ortamında (localhost) çakışma olmaması için false
                Path = "/",    // <--- ÇOK ÖNEMLİ: Tüm sayfalar bu çerezi görsün
                SameSite = SameSiteMode.Lax,
                Expires = DateTimeOffset.UtcNow.AddHours(2)
            });
            HttpContext.Items["jwtToken"] = authResult.Token;

            // ---------- COOKIE AUTH ----------
            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, authResult.User.Id.ToString()),
                new Claim(ClaimTypes.Name, $"{authResult.User.FirstName} {authResult.User.LastName}"),
                new Claim(ClaimTypes.Email, authResult.User.Email),
                new Claim(ClaimTypes.Role, authResult.User.RoleName),
                new Claim("Enabled", authResult.User.Enabled.ToString())
            };

            var identity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
            var authProperties = new AuthenticationProperties
            {
                IsPersistent = true,
                ExpiresUtc = DateTimeOffset.UtcNow.AddHours(2)
            };

            await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, new ClaimsPrincipal(identity), authProperties);


            // ---------- SESSION INFO ----------

            var userInfo = new ECommerce.Web.Mvc.Models.User.UserInformationViewModel
            {
                FullName = $"{authResult.User.FirstName} {authResult.User.LastName}",
                Email = authResult.User.Email
            };

            HttpContext.Session.SetString(
                "UserInfo",
                System.Text.Json.JsonSerializer.Serialize(userInfo));

            await MergeSessionCartToDbAsync(authResult.User.Id, authResult.Token);
            await MergeSessionFavoritesToDbAsync(authResult.User.Id, authResult.Token);

           
            return RedirectToLocal(returnUrl);
           
        }

        // ---------------- REGISTER ----------------
        [AllowAnonymous]
        [HttpGet("account/register")]
        public IActionResult Register()
        {
            if (IsLoggedIn())
                return RedirectToAction("Details", "Profile");

            return View();
        }

        [AllowAnonymous]
        [HttpPost("account/register")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Register(RegisterViewModel model)
        {
            if (!ModelState.IsValid)
                return View(model);

            // API üzerinden kontrol
            var existingUser = await _userApiService.GetByEmailAsync(model.Email);
            if (existingUser != null)
            {
                ModelState.AddModelError("", "This email is already taken.");
                return View(model);
            }

            // DTO oluşturma - Service metoduna uygun hale getirildi
            var registerDto = new UserDTO
            {
                Email = model.Email,
                FirstName = model.FirstName,
                LastName = model.LastName,
                Password = model.Password,
                Phone = model.Phone,
                Address = model.Address
            };
            var newUser = await _userApiService.RegisterAsync(registerDto);


            // Kayıt sonrası otomatik Login (Basitleştirilmiş)
            return RedirectToAction("Login");
        }

        // ---------------- LOGOUT ----------------
        [Authorize]
        [Route("account/logout")]
        public async Task<IActionResult> Logout()
        {
            //HttpContext.Session.Clear();
            SessionHelper.ClearCart(HttpContext);
            SessionHelper.ClearFavorites(HttpContext);

            Response.Cookies.Delete("Web_JwtToken");
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            return RedirectToAction("Login");
        }


        // ---------------- FORGOT PASSWORD ----------------
        [AllowAnonymous]
        [HttpGet("account/forgot-password")]
        public IActionResult ForgotPassword()
        {
            return View();
        }

        [AllowAnonymous]
        [HttpPost("account/forgot-password")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ForgotPassword(ForgotPasswordViewModel model)
        {
            if (!ModelState.IsValid)
                return View(model);

            var user = await _userApiService.GetByEmailAsync(model.Email);
            if (user == null)
            {
                ModelState.AddModelError("", "No account found with this email.");  
                return View(model); // Hata mesajı ve aynı sayfada kalma
            }

            // MVC artık kendi başına token üretmez! 
            // API'ye "Bu e-posta için şifre sıfırlama süreci başlat" diyoruz.
            // API içeride kullanıcıyı kontrol eder, token üretir ve Mail gönderir.
            var success = await _userApiService.InitiatePasswordResetAsync(model.Email);


            if (!success)
            {
                // Güvenlik gereği "Kullanıcı bulunamadı" demek yerine 
                // genellikle genel bir mesaj verilir ama şimdilik böyle olsun:
                ModelState.AddModelError("", "If an account exists for that email, a reset link has been sent.");
                return View(model);
            }

            TempData["Message"] = "A password reset link has been sent to your email address.";
            return RedirectToAction("Login");


        }

        // ---------------- RESET PASSWORD ----------------
        [AllowAnonymous]
        [HttpGet("account/reset-password")]
        public async Task<IActionResult> ResetPassword(string token)
        {
            if (string.IsNullOrEmpty(token))
            {
                TempData["Error"] = "Invalid token.";
                return RedirectToAction("ForgotPassword");
            }

            // Token'ı doğrula
            var isValid = await _userApiService.VerifyResetTokenAsync(token);
            if (!isValid)
            {
                TempData["Error"] = "The password reset link is invalid or has expired.";
                return RedirectToAction("ForgotPassword");
            }

            // Token geçerli ise, kullanıcıyı şifre sıfırlama ekranına yönlendir
            ViewBag.Token = token;
            return View();
        }

        [AllowAnonymous]
        [HttpPost("account/reset-password")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ResetPassword(ResetPasswordViewModel model, string token)
        {
            if (!ModelState.IsValid)
                return View(model);

            var result = await _userApiService.ResetPasswordAsync(token, model.NewPassword);

            if (result)
            {
                TempData["Message"] = "Your password has been reset successfully. You can now login.";
                return RedirectToAction("Login");
            }

            ModelState.AddModelError("", "An error occurred while resetting your password. Please try again.");
            return View(model);
        }


        // ---------------- HELPERS ----------------

        private IActionResult RedirectToLocal(string? returnUrl)
        {
            if (!string.IsNullOrEmpty(returnUrl) && Url.IsLocalUrl(returnUrl))
                return Redirect(returnUrl);

            return RedirectToAction("Details", "Profile");
        }


       
    }
}