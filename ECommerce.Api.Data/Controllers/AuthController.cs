using ECommerce.Application.DTOs.Auth;
using ECommerce.Application.DTOs.User;
using ECommerce.Application.Interfaces.Services;
using ECommerce.Application.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ECommerce.Api.Data.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly IUserService _userService;
        private readonly JWTService _jwtService;

        public AuthController(IUserService userService, JWTService jwtService)
        {
            _userService = userService;
            _jwtService = jwtService;
        }

        [AllowAnonymous]
        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginRequestDTO model)
        {
            // 1. UserService artık LoginResponseDTO dönüyor
            var loginResult = await _userService.AuthenticateAsync(model.Email, model.Password);
            if (loginResult == null)
                return Unauthorized(new { message = "Invalid email or password" });

            // 2. Token üretimi (Eğer UserService içinde üretmiyorsan burada üretip pakete ekleyebilirsin)
            var token = _jwtService.GenerateToken( loginResult.User.Id, $"{loginResult.User.FirstName} {loginResult.User.LastName}",
            loginResult.User.RoleName);

            loginResult.Token = token;

            // 3. Doğrudan DTO nesnesini dönüyoruz
            return Ok(loginResult);
        }

        [AllowAnonymous]
        [HttpGet("by-email")] // URL şuna dönüşür: api/auth/by-email
        public async Task<IActionResult> GetByEmail(string email)
        {
            var user = await _userService.GetByEmailAsync(email);
            if (user == null) return NotFound(); // Kullanıcı yoksa 404 döner, bu kayıt için uygundur.
            return Ok(user);
        }

        [AllowAnonymous]
        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] UserDTO dto)
        {
            // E-posta adresi kullanımda mı kontrolü
            var existingUser = await _userService.GetByEmailAsync(dto.Email);
            if (existingUser != null)
            {
                return BadRequest(new { message = "Email already in use." });
            }

            // DİKKAT: AddAsync yerine RegisterAsync kullanıyoruz çünkü o UserDTO dönüyor.
            var createdUser = await _userService.RegisterAsync(dto);

            if (createdUser == null)
            {
                return BadRequest(new { message = "Registration failed." });
            }

            return Ok(createdUser);
        }
    }

}
