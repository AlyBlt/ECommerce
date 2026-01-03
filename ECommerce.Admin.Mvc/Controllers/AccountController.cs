using ECommerce.Admin.Mvc.Filters;
using ECommerce.Admin.Mvc.Models.User;
using ECommerce.Application.DTOs.User;
using ECommerce.Application.Interfaces.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using System.Threading.Tasks;

namespace ECommerce.Admin.Mvc.Controllers
{
    [Route("admin/account")]
    [Authorize(Roles = "Admin")]
    [ActiveUserAuthorize]
    public class AccountController : Controller
    {
        private readonly IUserService _userService;

        public AccountController(IUserService userService)
        {
            _userService = userService;
        }

        // Profile sayfası 
        [HttpGet("profile")]
        public async Task<IActionResult> Profile()
        {
            var adminId = GetCurrentUserId();
            if (adminId == 0) return RedirectToAction("Login", "Auth");

            var adminDto = await _userService.GetAsync(adminId);
            if (adminDto == null) return RedirectToAction("Login", "Auth");

            // DTO -> ViewModel Mapping
            var model = MapToViewModel(adminDto);

            return View(model);
        }

        // Settings sayfası (GET)
        [HttpGet("settings")]
        public async Task<IActionResult> Settings()
        {
            var adminId = GetCurrentUserId();
            if (adminId == 0) return RedirectToAction("Login", "Auth");

            var adminDto = await _userService.GetAsync(adminId);
            if (adminDto == null) return RedirectToAction("Login", "Auth");

            var model = MapToViewModel(adminDto);

            return View(model);
        }

        // Settings sayfası (POST)
        [HttpPost("settings")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Settings(UserViewModel model)
        {
            if (!ModelState.IsValid)
                return View(model);

            // Mevcut veriyi servisten çekelim (E-posta değişikliği kontrolü vb. için gerekebilir)
            var adminDto = await _userService.GetAsync(model.Id);
            if (adminDto == null) return RedirectToAction("Login", "Auth");

            // ViewModel -> DTO Güncellemesi
            adminDto.FirstName = model.FirstName;
            adminDto.LastName = model.LastName;
            adminDto.Email = model.Email;
            adminDto.Phone = model.Phone;
            adminDto.Address = model.Address;

            // Servis üzerinden güncelleyelim
            await _userService.UpdateAsync(adminDto);

            TempData["SuccessMessage"] = "Settings saved successfully!";
            return View(model);
        }


        // --- Helper Methods ---
        private int GetCurrentUserId()
        {
            return int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? "0");
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
                RoleName = dto.RoleName // Artık Entity değil, DTO'dan gelen string isim
            };
        }
    }
}