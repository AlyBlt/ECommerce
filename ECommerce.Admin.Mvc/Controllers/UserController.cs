using ECommerce.Application.Filters;
using ECommerce.Admin.Mvc.Models.User;
using ECommerce.Application.Interfaces.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ECommerce.Admin.Mvc.Controllers
{
    [Route("admin/users")] // Route daha spesifik hale getirildi
    [Authorize(Policy = "AdminPanelAccess")]
    [ActiveUserAuthorize]
    public class UserController : Controller
    {
        private readonly IUserService _userApiService;

        public UserController(IUserService userApiService)
        {
            _userApiService = userApiService;
        }

        // GET: /admin/users/list
        [HttpGet("list")]
        public async Task<IActionResult> List()
        {
            // API'den "api/user/all" endpoint'ine istek gider
            var userDtos = await _userApiService.GetAllAsync();

            var vm = userDtos.Select(u => new UserViewModel
            {
                Id = u.Id,
                FirstName = u.FirstName,
                LastName = u.LastName,
                Email = u.Email,
                Enabled = u.Enabled,
                IsSellerApproved = u.IsSellerApproved,
                HasPendingSellerRequest = u.HasPendingSellerRequest,
                IsRejected = u.IsRejected,
                RoleName = u.RoleName
            }).OrderByDescending(u => u.HasPendingSellerRequest)
              .ThenBy(u => u.FirstName).ToList();

            return View(vm);
        }

        // GET: /admin/users/approve/{id}
        [HttpGet("approve/{id}")]
        public async Task<IActionResult> ApproveForm(int id)
        {
            // API'den "api/user/{id}" endpoint'ine istek gider
            var userDto = await _userApiService.GetAsync(id);
            if (userDto == null)
            {
                TempData["ErrorMessage"] = "User not found!";
                return RedirectToAction(nameof(List));
            }

            var model = MapToViewModel(userDto);
            return View("Approve", model);
        }

        // POST: /admin/users/approve/{id}
        [HttpPost("approve/{id}")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ApproveConfirmed(int id)
        {
            // API'ye "api/user/approve/{id}" POST isteği gider
            await _userApiService.ApproveSellerAsync(id);
            TempData["SuccessMessage"] = "Seller successfully approved!";
            return RedirectToAction(nameof(List));
        }

        // GET: /admin/users/reject/{id}
        [HttpGet("reject/{id}")]
        public async Task<IActionResult> RejectForm(int id)
        {
            var userDto = await _userApiService.GetAsync(id);
            if (userDto == null)
            {
                TempData["ErrorMessage"] = "User not found!";
                return RedirectToAction(nameof(List));
            }

            return View("Reject", MapToViewModel(userDto));
        }

        // POST: /admin/users/reject/{id}
        [HttpPost("reject/{id}")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> RejectConfirmed(int id)
        {
            // API'ye "api/user/reject/{id}" POST isteği gider
            await _userApiService.RejectSellerAsync(id);
            TempData["SuccessMessage"] = "Seller request rejected!";
            return RedirectToAction(nameof(List));
        }

        //----------------TOGGLE---------------

       // GET: /admin/users/toggle/{id} (Action adı GET olsa da API'de POST çalışır)
        [HttpGet("toggle/{id}")]
        public async Task<IActionResult> ToggleEnabled(int id)
        {
            try
            {
                // API'ye "api/user/toggle/{id}" POST isteği gider
                await _userApiService.ToggleEnabledAsync(id);
                TempData["SuccessMessage"] = "User status updated successfully!";
            }
            catch (Exception ex)
            {
                // API'den gelen "System Administrator cannot be deactivated" mesajını burada yakala
                TempData["ErrorMessage"] = ex.Message;
            }

            return RedirectToAction(nameof(List));
        }


        // --- Helper Mapping ---
        private UserViewModel MapToViewModel(ECommerce.Application.DTOs.User.UserDTO dto)
        {
            return new UserViewModel
            {
                Id = dto.Id,
                FirstName = dto.FirstName,
                LastName = dto.LastName,
                Email = dto.Email,
                Enabled = dto.Enabled,
                IsSellerApproved = dto.IsSellerApproved,
                HasPendingSellerRequest = dto.HasPendingSellerRequest,
                RoleName = dto.RoleName
            };
        }
    }
}