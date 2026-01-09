using ECommerce.Application.DTOs.User;
using ECommerce.Application.Interfaces.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace ECommerce.Api.Data.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize] // Kendi profilini yönettiği için giriş yapmış olması yeterli
    public class AccountController : ControllerBase
    {
        private readonly IUserService _userService;

        public AccountController(IUserService userService)
        {
            _userService = userService;
        }

        // GET: api/account/{id}
        // MVC tarafındaki Profile ve Settings (GET) aksiyonları için
        // GET: api/account/me
        [HttpGet("me")]
        public async Task<IActionResult> GetCurrentUser(bool includeOrders = false, bool includeProducts = false)
        {
            var userIdClaim = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(userIdClaim)) return Unauthorized();

            var currentUserId = int.Parse(userIdClaim);

            // BURASI KRİTİK: Parametreleri GetAsync'e geçiyoruz
            var user = await _userService.GetAsync(currentUserId, includeOrders, includeProducts);

            if (user == null) return NotFound("User not found.");
            return Ok(user);
        }

        // PUT: api/account/me
        [HttpPut("me")]
        public async Task<IActionResult> UpdateCurrentUser([FromBody] UserDTO dto)
        {
            var currentUserId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));
            if (dto.Id == 0) dto.Id = currentUserId;
            if (dto.Id != currentUserId) return Forbid();

            var user = await _userService.GetAsync(currentUserId);
            if (user == null) return NotFound();

            // Sadece null olmayan değerleri güncelle
            user.FirstName = dto.FirstName ?? user.FirstName;
            user.LastName = dto.LastName ?? user.LastName;
            user.Email = dto.Email ?? user.Email;
            user.Phone = dto.Phone ?? user.Phone;
            user.Address = dto.Address ?? user.Address;

            // Password null gelirse dokunma, dolu gelirse (şifre değiştirme yapılıyorsa) güncelle
            if (!string.IsNullOrEmpty(dto.Password))
            {
                user.Password = dto.Password; // Şifre hashleme mekanizman varsa burada olmalı
            }

            await _userService.UpdateAsync(user);
            return Ok(user);
        }

        [HttpPost("request-seller")]
        [Authorize(Roles = "Buyer")]
        public async Task<IActionResult> RequestSellerStatus()
        {
            // User.FindFirstValue(ClaimTypes.NameIdentifier) içindeki ID'nin 
            // JWT Token'dan doğru geldiğinden emin ol.
            var userIdClaim = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(userIdClaim)) return Unauthorized();

            var currentUserId = int.Parse(userIdClaim);
            await _userService.RequestSellerStatusAsync(currentUserId);
            return Ok(new { message = "Seller request submitted." });
        }
    }
}

