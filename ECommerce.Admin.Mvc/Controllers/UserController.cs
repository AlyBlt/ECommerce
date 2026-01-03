using ECommerce.Admin.Mvc.Filters;
using ECommerce.Admin.Mvc.Models.User;
using ECommerce.Application.Interfaces.Services;
using ECommerce.Application.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ECommerce.Admin.Mvc.Controllers
{
    [Route("admin/users")] // Route daha spesifik hale getirildi
    [Authorize(Roles = "Admin")]
    [ActiveUserAuthorize]
    public class UserController : Controller
    {
        private readonly IUserService _userService;
      
        public UserController(IUserService userService)
        {
            _userService = userService;
        }

        // GET: /admin/users/list
        [HttpGet("list")]
        public async Task<IActionResult> List()
        {
            var userdto = await _userService.GetAllAsync();

            // DTO -> ViewModel Mapping
            var vm = userdto.Select(u => new UserViewModel
            {
                Id = u.Id,
                FirstName = u.FirstName,
                LastName = u.LastName,
                Email = u.Email,
                Enabled = u.Enabled,
                IsSellerApproved = u.IsSellerApproved,
                HasPendingSellerRequest = u.HasPendingSellerRequest,
                IsRejected = u.IsRejected,
                RoleName = u.RoleName // Servisten gelen rol adını ekledik
            }).OrderByDescending(u => u.HasPendingSellerRequest)
            .ThenBy(u => u.FirstName).ToList();

            return View(vm);
        }

        // GET: /admin/users/approve/5
        [HttpGet("approve/{id}")]
        public async Task<IActionResult> ApproveForm(int id)
        {
            var userDto = await _userService.GetAsync(id);
            if (userDto == null)
            {
                TempData["ErrorMessage"] = "User not found!";
                return RedirectToAction(nameof(List));
            }

            var model = new UserViewModel
            {
                Id = userDto.Id,
                FirstName = userDto.FirstName,
                LastName = userDto.LastName,
                Email = userDto.Email,
                Enabled = userDto.Enabled,
                IsSellerApproved = userDto.IsSellerApproved,
                HasPendingSellerRequest = userDto.HasPendingSellerRequest,
                RoleName = userDto.RoleName
            };

            return View("Approve", model);
        }

        // POST: /admin/users/approve/5
        [HttpPost("approve/{id}")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ApproveConfirmed(int id)
        {
            var userDto = await _userService.GetAsync(id);
            if (userDto == null)
            {
                TempData["ErrorMessage"] = "User not found!"; 
                return RedirectToAction(nameof(List));
            }
           
            await _userService.ApproveSellerAsync(id); 
            TempData["SuccessMessage"] = "Seller successfully approved!";  
            return RedirectToAction(nameof(List));
        }

        // GET: /admin/users/reject/5
        [HttpGet("reject/{id}")]
        public async Task<IActionResult> RejectForm(int id)
        {
            var userDto = await _userService.GetAsync(id);
            if (userDto == null)
            {
                TempData["ErrorMessage"] = "User not found!";
                return RedirectToAction(nameof(List));
            }

            var model = new UserViewModel
            {
                Id = userDto.Id,
                FirstName = userDto.FirstName,
                LastName = userDto.LastName,
                Email = userDto.Email,
                Enabled = userDto.Enabled,
                IsSellerApproved = userDto.IsSellerApproved,
                HasPendingSellerRequest = userDto.HasPendingSellerRequest,
                IsRejected = userDto.IsRejected
            };

            return View("Reject", model); 
        }

        // POST: /admin/users/reject/5
        [HttpPost("reject/{id}")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> RejectConfirmed(int id)
        {
            await _userService.RejectSellerAsync(id);

            TempData["SuccessMessage"] = "Seller request rejected!";
            return RedirectToAction(nameof(List));
        }


        //----------------TOGGLE---------------

        // GET: /admin/users/toggle/5
        [HttpGet("toggle/{id}")]
        public async Task<IActionResult> ToggleEnabled(int id)
        {
            var userDto = await _userService.GetAsync(id);
            if (userDto == null)
            {
                TempData["ErrorMessage"] = "User not found!";
                return RedirectToAction(nameof(List));
            }

            // Toggle işlemi servise bırak
            await _userService.ToggleEnabledAsync(id);

            // DB’den güncel kullanıcıyı tekrar çek
            userDto = await _userService.GetAsync(id);

            TempData["SuccessMessage"] = userDto.Enabled
                ? "User activated successfully!"
                : "User deactivated successfully!";

            return RedirectToAction(nameof(List));
        }
    }
}