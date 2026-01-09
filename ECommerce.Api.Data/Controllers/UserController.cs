using ECommerce.Application.DTOs.Auth;
using ECommerce.Application.DTOs.User;
using ECommerce.Application.Interfaces.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity.Data;
using Microsoft.AspNetCore.Mvc;

namespace ECommerce.Api.Data.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(Policy = "AdminPanelAccess")]
    public class UserController : ControllerBase
    {
        private readonly IUserService _userService;

        public UserController(IUserService userService)
        {
            _userService = userService;
        }

        // GET: api/user/all (MVC'deki List aksiyonu için)
        [HttpGet("all")]
        public async Task<IActionResult> GetAll()
        {
            var users = await _userService.GetAllAsync();
            return Ok(users);
        }

        // GET: api/user/{id} (MVC'deki ApproveForm, RejectForm ve Toggle için veri çekme)
        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            var user = await _userService.GetAsync(id);
            if (user == null) return NotFound("User not found!");
            return Ok(user);
        }

        // POST: api/user/approve/{id} (MVC'deki ApproveConfirmed için)
        [HttpPost("approve/{id}")]
        public async Task<IActionResult> ApproveSeller(int id)
        {
            var user = await _userService.GetAsync(id);
            if (user == null) return NotFound();

            await _userService.ApproveSellerAsync(id);
            return Ok(new { Message = "Seller successfully approved!" });
        }

        // POST: api/user/reject/{id} (MVC'deki RejectConfirmed için)
        [HttpPost("reject/{id}")]
        public async Task<IActionResult> RejectSeller(int id)
        {
            var user = await _userService.GetAsync(id);
            if (user == null) return NotFound();

            await _userService.RejectSellerAsync(id);
            return Ok(new { Message = "Seller request rejected!" });
        }

        // POST: api/user/toggle/{id} (MVC'deki ToggleEnabled için)
        [HttpPost("toggle/{id}")]
        public async Task<IActionResult> ToggleEnabled(int id)
        {
            var user = await _userService.GetAsync(id);
            if (user == null) return NotFound();

            // SystemAdmin pasif yapılamaz!
            if (user.RoleName == "SystemAdmin")
            {
                return BadRequest(new { Message = "System Administrator cannot be deactivated or modified!" });
            }

            await _userService.ToggleEnabledAsync(id);

            // Güncel durumu geri dönelim ki MVC tarafı ne olduğunu bilsin
            var updatedUser = await _userService.GetAsync(id);
            return Ok(updatedUser);
        }

        [HttpPost("initiate-password-reset")]
        [AllowAnonymous] // Şifre unutan kullanıcı henüz login değildir!
        public async Task<IActionResult> InitiatePasswordReset([FromBody] ForgotPasswordRequest request)
        {
            var result = await _userService.InitiatePasswordResetAsync(request.Email);
            if (result) return Ok(new { Message = "Reset email sent." });
            return BadRequest("User not found or email could not be sent.");
        }

        [HttpPost("reset-password")]
        [AllowAnonymous]
        public async Task<IActionResult> ResetPassword([FromBody] ResetPasswordRequestDTO request)
        {
            var result = await _userService.ResetPasswordAsync(request.Token, request.NewPassword);
            if (result) return Ok(new { Message = "Password reset successful." });
            return BadRequest("Invalid token or expired.");
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateCurrentUser(int id, [FromBody] UserDTO dto)
        {
            // id parametresi URL'den (api/user/5 gibi) otomatik gelir.
            if (id == 0) id = dto.Id;

            var user = await _userService.GetAsync(id);
            if (user == null) return NotFound();

            // 3. Değerleri aktar
            user.FirstName = dto.FirstName ?? user.FirstName;
            user.LastName = dto.LastName ?? user.LastName;
            user.Email = dto.Email ?? user.Email;
            user.Phone = dto.Phone ?? user.Phone;
            user.Address = dto.Address ?? user.Address;
           
            if (!string.IsNullOrEmpty(dto.Password))
            {
                user.Password = dto.Password;
            }

            await _userService.UpdateAsync(user);

            // Güncellenmiş nesneyi dön
            return Ok(user);
        }
    }
}
