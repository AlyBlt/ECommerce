using ECommerce.Application.Filters;
using ECommerce.Admin.Mvc.Models.User;
using ECommerce.Application.DTOs.User;
using ECommerce.Application.Interfaces.Services;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace ECommerce.Admin.Mvc.Controllers
{
    [Route("admin/account")]
    [Authorize(Policy = "AdminPanelAccess")]
    [ActiveUserAuthorize]
    public class AccountController : Controller
    {
        private readonly IUserService _userApiService;

        public AccountController(IUserService userApiService)
        {
            _userApiService = userApiService;
        }

        // Profile sayfası 
        [HttpGet("profile")]
        public async Task<IActionResult> Profile()
        {
            var adminId = GetCurrentUserId();
            if (adminId == 0) return RedirectToAction("Login", "Auth");

            // UserApiService içindeki GetAsync metodunu kullanıyoruz
            // Bu metot arka planda HttpClient ile "api/user/{id}" adresine gider
            var adminDto = await _userApiService.GetAsync(adminId);

            if (adminDto == null) return RedirectToAction("Login", "Auth");

            return View(MapToViewModel(adminDto));
        }

        [HttpGet("settings")]
        public async Task<IActionResult> Settings()
        {
            
            var adminDto = await _userApiService.GetCurrentUserAsync(GetCurrentUserId());
            if (adminDto == null) return RedirectToAction("Login", "Auth");

            return View(MapToViewModel(adminDto));
        }

        // Settings sayfası (POST)
        [HttpPost("settings")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Settings(UserViewModel model)
        {
            ModelState.Remove("RoleName");
          
            if (!ModelState.IsValid)
            {
                return View(model);
            }

           
            try
            {
                var currentUserId = GetCurrentUserId();
                var adminDto = await _userApiService.GetAsync(currentUserId);

                if (adminDto == null)
                {
                    ModelState.AddModelError("", "User not found.");
                    return View(model);
                }

                // Map values from ViewModel to DTO
                adminDto.FirstName = model.FirstName;
                adminDto.LastName = model.LastName;
                adminDto.Email = model.Email;
                adminDto.Phone = model.Phone;
                adminDto.Address = model.Address;
               

                // Call the API service to update
                await _userApiService.UpdateAsync(adminDto);

                await UpdateUserCookie(adminDto.FirstName + " " + adminDto.LastName);

                // Success notification
                TempData["SuccessMessage"] = "Profile settings updated successfully!";

                return RedirectToAction(nameof(Settings));
            }
            catch (Exception ex)
            {
                // Error notification with exception details
                ModelState.AddModelError("", "An error occurred while updating the profile: " + ex.Message);
                return View(model);
            }
        }


        // --- Helper Methods ---
        private int GetCurrentUserId()
        {
            var userIdClaim = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(userIdClaim)) return 0; // Veya hata fırlat
            return int.Parse(userIdClaim);
        }

        private UserViewModel MapToViewModel(UserDTO dto)
        {
            return new UserViewModel
            {
                Id = dto.Id,
                FirstName = dto.FirstName,
                LastName = dto.LastName,
                Email = dto.Email,
                Phone = dto.Phone,
                Address = dto.Address,
                RoleName = dto.RoleName
            };
        }

        private async Task UpdateUserCookie(string newFullName)
        {
            var identity = (ClaimsIdentity)User.Identity!;

            // Eski Name claim'ini bul ve kaldır
            var existingClaim = identity.FindFirst(ClaimTypes.Name);
            if (existingClaim != null)
            {
                identity.RemoveClaim(existingClaim);
            }

            // Yeni ismi ekle
            identity.AddClaim(new Claim(ClaimTypes.Name, newFullName));

            // Yeni kimlik bilgisiyle çerezi tekrar yaz (Login işlemine benzer)
            var principal = new ClaimsPrincipal(identity);
            await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, principal, new AuthenticationProperties
            {
                IsPersistent = true // Kullanıcının "beni hatırla" ayarı korunsun
            });
        }
    }
}