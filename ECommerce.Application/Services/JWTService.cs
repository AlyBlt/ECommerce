using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.Extensions.Configuration;

namespace ECommerce.Application.Services
{
    public class JWTService
    {
        private readonly IConfiguration _configuration;

        public JWTService(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public string GenerateToken(int userId, string userName, string role)
        {
            var claims = new[]
            {
                new Claim(ClaimTypes.NameIdentifier, userId.ToString()),
                new Claim(ClaimTypes.Name, userName),
                new Claim(ClaimTypes.Role, role),
                // Yeni eklenen kısım:
               new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
            };

            var key = new SymmetricSecurityKey(
                Encoding.UTF8.GetBytes(_configuration["Jwt:SecretKey"])
            );

            var token = new JwtSecurityToken(
                issuer: _configuration["Jwt:Issuer"],
                audience: _configuration["Jwt:Audience"],
                claims: claims,
                expires: DateTime.Now.AddDays(1),
                signingCredentials: new SigningCredentials(key, SecurityAlgorithms.HmacSha256)
            );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }

        // Şifre sıfırlama için özel token oluşturma
        public string GeneratePasswordResetToken(int userId, string email)
        {
            var claims = new[]
            {
                new Claim(ClaimTypes.NameIdentifier, userId.ToString()),
                new Claim(ClaimTypes.Email, email),
                new Claim("purpose", "password-reset")  // Şifre sıfırlama amacını belirtelim
            };

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["Jwt:SecretKey"]));
            var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
            var expirationTime = DateTime.Now.AddHours(1); // Token geçerlilik süresi 1 saat

            var token = new JwtSecurityToken(
                issuer: _configuration["Jwt:Issuer"],
                audience: _configuration["Jwt:Audience"],
                claims: claims,
                expires: expirationTime,
                signingCredentials: credentials
            );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }

        public bool ValidatePasswordResetToken(string token)
        {
            var handler = new JwtSecurityTokenHandler();
            try
            {
                var jsonToken = handler.ReadToken(token) as JwtSecurityToken;
                var expirationClaim = jsonToken?.Claims.FirstOrDefault(c => c.Type == "exp");

                // Token'ın süresi dolmuşsa, geçersiz kabul et
                if (expirationClaim != null && DateTime.UtcNow > DateTime.Parse(expirationClaim.Value))
                {
                    return false;  // Token süresi geçmiş
                }

                // Token'ın amacının "password-reset" olup olmadığını kontrol et
                var purposeClaim = jsonToken?.Claims.FirstOrDefault(c => c.Type == "purpose" && c.Value == "password-reset");
                return purposeClaim != null;  // Eğer purpose "password-reset" ise geçerli kabul et
            }
            catch
            {
                return false;  // Token geçersiz
            }
        }


        // JWT Token'ından UserId almak
        public int GetUserIdFromJwt(string token)
        {
            if (string.IsNullOrEmpty(token))
            {
                return 0;
            }

            var handler = new JwtSecurityTokenHandler();
            try
            {
                var jsonToken = handler.ReadToken(token) as JwtSecurityToken;
                var userIdClaim = jsonToken?.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier);

                if (userIdClaim != null && int.TryParse(userIdClaim.Value, out int userId))
                {
                    return userId;
                }
            }
            catch
            {
                return 0; // Token çözülürken hata olursa 0 döner
            }

            return 0;
        }

        public string GetUserNameFromJwt(string token)
        {
            if (string.IsNullOrEmpty(token)) return "";

            var handler = new JwtSecurityTokenHandler();
            try
            {
                var jsonToken = handler.ReadToken(token) as JwtSecurityToken;
                var nameClaim = jsonToken?.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Name);
                return nameClaim?.Value ?? "";
            }
            catch
            {
                return "";
            }
        }
    }

}