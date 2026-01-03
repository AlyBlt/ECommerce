using ECommerce.Application.DTOs.Cart;
using ECommerce.Application.DTOs.User;
using ECommerce.Application.Interfaces.Services;
using ECommerce.Application.Services;
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
        private readonly IUserService _userService;
        private readonly ICartService _cartService;
        private readonly IFavoriteService _favoriteService;
        private readonly IEmailService _emailService;
        private readonly JWTService _jwtService;

        public AuthController(
            IUserService userService,
            ICartService cartService,
            IFavoriteService favoriteService,
            IEmailService emailService,
            JWTService jwtService)
        {
            _userService = userService;
            _cartService = cartService;
            _favoriteService = favoriteService;
            _emailService = emailService;
            _jwtService = jwtService;
        }

        private bool IsLoggedIn() => User.Identity?.IsAuthenticated == true;

        // ---------------- MERGE METHODS ----------------

        private async Task MergeSessionCartToDbAsync(int userId)
        {
            var sessionCart = SessionHelper.GetCart(HttpContext);
            if (sessionCart != null && sessionCart.Items.Any())
            {
                foreach (var item in sessionCart.Items)
                {
                    // Yeni DTO yapısına uygun nesne oluşturuyoruz
                    var cartDto = new CartAddDTO
                    {
                        ProductId = item.ProductId,
                        Quantity = item.Quantity
                        // ImageUrl artık servisin içinde Product üzerinden çekiliyor
                    };

                    await _cartService.AddToCartAsync(userId, cartDto);
                }

                SessionHelper.ClearCart(HttpContext);

                var cartItems = await _cartService.GetCartItemsAsync(userId);
                HttpContext.Session.SetInt32(
                    "CartCount",
                    cartItems.Sum(x => x.Quantity));
            }
        }

        private async Task MergeSessionFavoritesToDbAsync(int userId)
        {
            var sessionFavs = SessionHelper.GetFavorites(HttpContext);
            if (sessionFavs != null && sessionFavs.Any())
            {
                foreach (var fav in sessionFavs)
                {
                    await _favoriteService.AddAsync(userId, fav.ProductId);
                }

                SessionHelper.ClearFavorites(HttpContext);

                var favs = await _favoriteService.GetByUserAsync(userId);
                HttpContext.Session.SetInt32(
                    "FavoritesCount",
                    favs.Count());
            }
        }

        // ---------------- LOGIN ----------------
        [AllowAnonymous]
        [HttpGet("account/login")]
        public IActionResult Login(bool passive = false, string ? returnUrl=null)
        {
            if (passive)
            {
                ViewBag.PassiveMessage =
                    "Your account has been deactivated by an administrator.";
            }

            if (IsLoggedIn())
                return RedirectToLocal(returnUrl);

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

            var userDto = await _userService.AuthenticateAsync(
                model.Email,
                model.Password);

            if (userDto == null)
            {
                ModelState.AddModelError("", "Invalid email or password.");
                return View(model);
            }

            if (!userDto.Enabled)
            {
                ModelState.AddModelError(
                    "",
                    "Your account is currently disabled. Please contact support.");
                return View(model);
            }


            // ---------- COOKIE AUTH ----------
            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, userDto.Id.ToString()),
                new Claim(ClaimTypes.Name, $"{userDto.FirstName} {userDto.LastName}"),
                new Claim(ClaimTypes.Email, userDto.Email),
                new Claim(ClaimTypes.Role, userDto.RoleName),
                new Claim("Enabled", userDto.Enabled.ToString())
            };

            var identity = new ClaimsIdentity(
                claims,
                CookieAuthenticationDefaults.AuthenticationScheme);

            await HttpContext.SignInAsync(
                CookieAuthenticationDefaults.AuthenticationScheme,
                new ClaimsPrincipal(identity));

            // ---------- SESSION INFO ----------
            
            var userInfo = new ECommerce.Web.Mvc.Models.User.UserInformationViewModel
            {
                FullName = $"{userDto.FirstName} {userDto.LastName}",
                Email = userDto.Email
            };

            HttpContext.Session.SetString(
                "UserInfo",
                System.Text.Json.JsonSerializer.Serialize(userInfo));

            await MergeSessionCartToDbAsync(userDto.Id);
            await MergeSessionFavoritesToDbAsync(userDto.Id);

            //HttpContext.Session.Clear();
            SessionHelper.ClearCart(HttpContext);
            SessionHelper.ClearFavorites(HttpContext);

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

            var existingUserDto = await _userService.GetByEmailAsync(model.Email);
            if (existingUserDto != null)
            {
                ModelState.AddModelError("", "This email address is already in use.");
                return View(model); // Hata varsa aynı sayfada kalacak
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
            var newUserDto = await _userService.RegisterAsync(registerDto);


            // Cookie Auth SignIn
            var claims = new List<Claim>
    {
        new Claim(ClaimTypes.NameIdentifier, newUserDto.Id.ToString()),
        new Claim(ClaimTypes.Name, $"{newUserDto.FirstName} {newUserDto.LastName}"),
        new Claim(ClaimTypes.Email, newUserDto.Email),
        new Claim(ClaimTypes.Role, "Buyer"),
        new Claim("Enabled", newUserDto.Enabled.ToString())

    };

            var identity = new ClaimsIdentity(
                claims,
                CookieAuthenticationDefaults.AuthenticationScheme);

            await HttpContext.SignInAsync(
                CookieAuthenticationDefaults.AuthenticationScheme,
                new ClaimsPrincipal(identity));

            // User bilgilerini session'a kaydetme
            var userInfo = new ECommerce.Web.Mvc.Models.User.UserInformationViewModel
            {
                FullName = $"{model.FirstName} {model.LastName}",
                Email = model.Email
            };

            HttpContext.Session.SetString(
                "UserInfo",
                System.Text.Json.JsonSerializer.Serialize(userInfo));

            await MergeSessionCartToDbAsync(newUserDto.Id);
            await MergeSessionFavoritesToDbAsync(newUserDto.Id);

            //HttpContext.Session.Clear();
            SessionHelper.ClearCart(HttpContext);
            SessionHelper.ClearFavorites(HttpContext);

            // Success message
            TempData["Success"] = "Your account has been successfully created.";
           // TempData.Keep("Success");

            return RedirectToAction("Details", "Profile");
        }

        // ---------------- LOGOUT ----------------
        [Authorize]
        [Route("account/logout")]
        public async Task<IActionResult> Logout()
        {
            //HttpContext.Session.Clear();
            SessionHelper.ClearCart(HttpContext);
            SessionHelper.ClearFavorites(HttpContext);

            await HttpContext.SignOutAsync(
                CookieAuthenticationDefaults.AuthenticationScheme);

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

            var user = await _userService.GetByEmailAsync(model.Email);
            if (user == null)
            {
                ModelState.AddModelError("", "No account found with this email.");  
                return View(model); // Hata mesajı ve aynı sayfada kalma
            }

            // Token generation logic
            var resetToken = _jwtService.GeneratePasswordResetToken(user.Id, user.Email);
            var resetLink = Url.Action("ResetPassword", "Auth", new { token = resetToken }, protocol: Request.Scheme);


            try
            {
                // E-posta ile reset linkini gönder
                await _emailService.SendPasswordResetEmail(user.Email, resetLink);
                TempData["Message"] = "A password reset link has been sent to your email.";
                TempData.Keep("Message");
                return RedirectToAction("Login"); // Burada kullanıcıyı Login sayfasına yönlendiriyoruz, başarıyla işlemi tamamladı
            }
            catch (Exception ex)
            {
                // Eğer e-posta gönderiminde bir hata alırsak
                ModelState.AddModelError("", "An error occurred while sending the email.");
                Debug.WriteLine($"Email send error: {ex.Message}");
                return View(model); // Hata oluştuğunda aynı sayfada tut
            }
           
            
        }

        // ---------------- RESET PASSWORD ----------------
        [AllowAnonymous]
        [HttpGet("account/reset-password")]
        public IActionResult ResetPassword(string token)
        {
            if (string.IsNullOrEmpty(token))
            {
                TempData["Error"] = "Invalid token.";
                return RedirectToAction("ForgotPassword");
            }

            // Token'ı doğrula
            var isValidToken = _jwtService.ValidatePasswordResetToken(token);
            if (!isValidToken)
            {
                TempData["Error"] = "Invalid or expired token.";
                return RedirectToAction("ForgotPassword");
            }

            // Token geçerli ise, kullanıcıyı şifre sıfırlama ekranına yönlendir
            return View();
        }

        [AllowAnonymous]
        [HttpPost("account/reset-password")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ResetPassword(ResetPasswordViewModel model, string token)
        {
            if (!ModelState.IsValid)
                return View(model);

            // Token'ı doğrula
            var isValidToken = _jwtService.ValidatePasswordResetToken(token);
            if (!isValidToken)
            {
                TempData["Error"] = "Invalid or expired token.";
                return RedirectToAction("ForgotPassword");
            }

            var userId = _jwtService.GetUserIdFromJwt(token);
            var userDto = await _userService.GetAsync(userId);

            if (userDto == null)
            {
                TempData["Error"] = "User not found.";
                return RedirectToAction("ForgotPassword");
            }

            // Şifreyi güncelle
            userDto.Password = model.NewPassword; // Şifreyi hashlemek isteyebiliriz ilerde!!
            await _userService.UpdateAsync(userDto);

            TempData["Message"] = "Password has been reset successfully.";
            return RedirectToAction("Login");
        }


        // ---------------- HELPERS ----------------

        private IActionResult RedirectToLocal(string? returnUrl)
        {
           if (!string.IsNullOrEmpty(returnUrl) && Url.IsLocalUrl(returnUrl))
           {
                // Sadece GET endpointlerini izin ver
              if (!returnUrl.StartsWith("/Product/Comment", StringComparison.OrdinalIgnoreCase))
              {
                  return Redirect(returnUrl);
               }
           }

                      // POST veya diğer güvenli olmayan URL'ler için default sayfa
                   return RedirectToAction("Details", "Profile");
        }
    }
}