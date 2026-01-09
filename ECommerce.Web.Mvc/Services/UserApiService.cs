using ECommerce.Application.DTOs.Auth;
using ECommerce.Application.DTOs.User;
using ECommerce.Application.Interfaces.Services;
using System.Text.Json;

namespace ECommerce.Web.Mvc.Services
{
    public class UserApiService : IUserService
    {
        private readonly HttpClient _httpClient;

        public UserApiService(IHttpClientFactory httpClientFactory)
        {
            _httpClient = httpClientFactory.CreateClient("DataApi");
        }

        public async Task<IEnumerable<UserDTO>> GetAllAsync()
        {
            return await _httpClient.GetFromJsonAsync<IEnumerable<UserDTO>>("api/user")
                   ?? new List<UserDTO>();
        }

        public async Task<UserDTO?> GetAsync(int id, bool includeOrders = false, bool includeProducts = false)
        {
            // API'ye parametreleri QueryString olarak gönderiyoruz
            // 1. Mevcut kullanıcının Token'ını HttpContext üzerinden al
            // Eğer token Claim olarak saklandıysa oradan çekilmeli.
            var url = $"api/user/{id}?includeOrders={includeOrders}&includeProducts={includeProducts}";
            

            var response = await _httpClient.GetAsync(url);

            if (response.StatusCode == System.Net.HttpStatusCode.Forbidden)
            {
                // Buraya düşüyorsa API "Bu ID'li kullanıcıyı görmeye yetkin yok" diyordur.
                return null;
            }

            return await response.Content.ReadFromJsonAsync<UserDTO>();
        }

        public async Task<UserDTO> GetByEmailAsync(string email)
        {
            // "api/user/by-email" olan yeri "api/auth/by-email" olarak güncelledik (API'deki konuma göre)
            var response = await _httpClient.GetAsync($"api/auth/by-email?email={email}");

            if (response.StatusCode == System.Net.HttpStatusCode.NotFound)
                return null; // Kullanıcı bulunamadıysa null dön ki Register devam edebilsin.

            response.EnsureSuccessStatusCode();
            return await response.Content.ReadFromJsonAsync<UserDTO>();
        }

        public async Task AddAsync(UserDTO dto)
        {
            var response = await _httpClient.PostAsJsonAsync("api/user", dto);
            response.EnsureSuccessStatusCode();
        }

        public async Task UpdateAsync(UserDTO dto)
        {
            var response = await _httpClient.PutAsJsonAsync($"api/user/{dto.Id}", dto);
            response.EnsureSuccessStatusCode();
        }

        public async Task DeleteAsync(int id)
        {
            var response = await _httpClient.DeleteAsync($"api/user/{id}");
            response.EnsureSuccessStatusCode();
        }

        public async Task<UserDTO> RegisterAsync(UserDTO dto)
        {
            // URL'nin api/auth/register olduğundan emin ol (api/user/register değil!)
            var response = await _httpClient.PostAsJsonAsync("api/auth/register", dto);

            if (!response.IsSuccessStatusCode)
            {
                var error = await response.Content.ReadAsStringAsync();
                throw new Exception($"API Error: {response.StatusCode} - {error}");
            }

            return await response.Content.ReadFromJsonAsync<UserDTO>();
        }

        public async Task<LoginResponseDTO?> AuthenticateAsync(string email, string password)
        {
            // API'ye login isteği atıyoruz
            var response = await _httpClient.PostAsJsonAsync("api/auth/login", new { email, password });

            if (!response.IsSuccessStatusCode) return null;

            // API'den gelen JSON'u okuyoruz
            var content = await response.Content.ReadAsStringAsync();

            // Büyük/küçük harf farkını (enabled vs Enabled) görmezden gel diyoruz
            var options = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };

            // Deserialization işlemini bu opsiyonla yapıyoruz
            var result = JsonSerializer.Deserialize<LoginResponseDTO>(content, options);

            return result;
        }

        public async Task ApproveSellerAsync(int id)
        {
            var response = await _httpClient.PostAsync($"api/user/approve-seller/{id}", null);
            response.EnsureSuccessStatusCode();
        }

        public async Task RejectSellerAsync(int id)
        {
            var response = await _httpClient.PostAsync($"api/account/request-seller/{id}", null);
            response.EnsureSuccessStatusCode();
        }

        // 1. Admin birini seller yapmak isterse (Kullanıcı Listesi ekranından)
        public async Task RequestSellerStatusAsync(int userId)
        {
            // Eğer API'de Admin için özel bir endpoint yoksa 
            // bunu [HttpPost("api/user/request-seller/{userId}")] gibi bir yere atabilirsin.
            var response = await _httpClient.PostAsync($"api/user/request-seller/{userId}", null);
            response.EnsureSuccessStatusCode();
        }

        // 2. Kullanıcı "Satıcı Ol" butonuna basarsa
        public async Task RequestSellerStatusAsync()
        {
            // Burası API'deki AccountController'a gider. 
            // ID gönderilmez, API bunu Token'dan (User.Identity) anlar.
            var response = await _httpClient.PostAsync("api/account/request-seller", null);
            response.EnsureSuccessStatusCode();
        }

        public async Task ToggleEnabledAsync(int userId)
        {
            var response = await _httpClient.PostAsync($"api/user/toggle/{userId}", null);
            response.EnsureSuccessStatusCode();
        }

        public async Task<bool> InitiatePasswordResetAsync(string email)
        {
            // API tarafında: [HttpPost("forgot-password")]
            var response = await _httpClient.PostAsJsonAsync("api/auth/forgot-password", new { email });
            return response.IsSuccessStatusCode;
        }

        public async Task<bool> VerifyResetTokenAsync(string token)
        {
            // API tarafında: [HttpGet("verify-token")]
            // Token genelde query string olarak gönderilir
            var response = await _httpClient.GetAsync($"api/auth/verify-token?token={token}");
            return response.IsSuccessStatusCode;
        }

        public async Task<bool> ResetPasswordAsync(string token, string newPassword)
        {
            // API tarafında: [HttpPost("reset-password")]
            var response = await _httpClient.PostAsJsonAsync("api/auth/reset-password", new { token, newPassword });
            return response.IsSuccessStatusCode;
        }

        public async Task<UserDTO?> GetCurrentUserAsync(int userId, bool includeOrders = false, bool includeProducts = false)
        {
            // URL'ye parametreleri ekliyoruz
            var url = $"api/account/me?includeOrders={includeOrders}&includeProducts={includeProducts}";

            var response = await _httpClient.GetAsync(url);
            if (!response.IsSuccessStatusCode) return null;

            return await response.Content.ReadFromJsonAsync<UserDTO>();
        }

        public async Task UpdateCurrentUserAsync(UserDTO dto)
        {
            var response = await _httpClient.PutAsJsonAsync("api/account/me", dto);
            response.EnsureSuccessStatusCode();
        }
    }
    
}
