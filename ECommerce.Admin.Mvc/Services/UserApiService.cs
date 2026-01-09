using ECommerce.Application.DTOs.Auth;
using ECommerce.Application.DTOs.User;
using ECommerce.Application.Interfaces.Services;

namespace ECommerce.Admin.Mvc.Services
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
            return await _httpClient.GetFromJsonAsync<IEnumerable<UserDTO>>("api/user/all")
                   ?? new List<UserDTO>();
        }

        public async Task<UserDTO?> GetAsync(int id, bool includeOrders = false, bool includeProducts = false)
        {
            // API'ye parametreleri QueryString olarak gönderiyoruz
            var url = $"api/user/{id}?includeOrders={includeOrders}&includeProducts={includeProducts}";
            var response = await _httpClient.GetAsync(url);
            if (!response.IsSuccessStatusCode) return null;

            return await response.Content.ReadFromJsonAsync<UserDTO>();
        }

        public async Task<UserDTO?> GetByEmailAsync(string email)
        {
            return await _httpClient.GetFromJsonAsync<UserDTO>($"api/user/by-email?email={email}");
        }

        public async Task AddAsync(UserDTO dto)
        {
            var response = await _httpClient.PostAsJsonAsync("api/user", dto);
            response.EnsureSuccessStatusCode();
        }

        public async Task UpdateAsync(UserDTO dto)
        {
            var response = await _httpClient.PutAsJsonAsync($"api/user/{dto.Id}", dto);

            // Eğer API 200 dönmezse burada hata fırlatacak ve AccountController'daki catch bloğuna düşecek.
            response.EnsureSuccessStatusCode();
        }

        public async Task DeleteAsync(int id)
        {
            var response = await _httpClient.DeleteAsync($"api/user/{id}");
            response.EnsureSuccessStatusCode();
        }

        public async Task<UserDTO> RegisterAsync(UserDTO dto)
        {
            var response = await _httpClient.PostAsJsonAsync("api/user/register", dto);
            response.EnsureSuccessStatusCode();
            return await response.Content.ReadFromJsonAsync<UserDTO>() ?? dto;
        }

        public async Task<LoginResponseDTO?> AuthenticateAsync(string email, string password)
        {
            var response = await _httpClient.PostAsJsonAsync("api/auth/login", new { Email = email, Password = password });

            if (response.IsSuccessStatusCode)
            {
                return await response.Content.ReadFromJsonAsync<LoginResponseDTO>();
            }
            return null;
        }

        public async Task ApproveSellerAsync(int id)
        {
            var response = await _httpClient.PostAsync($"api/user/approve/{id}", null);
            response.EnsureSuccessStatusCode();
        }

        public async Task RejectSellerAsync(int id)
        {
            var response = await _httpClient.PostAsync($"api/user/reject/{id}", null);
            response.EnsureSuccessStatusCode();
        }

        public async Task RequestSellerStatusAsync(int userId)
        {
            var response = await _httpClient.PostAsync($"api/user/request-seller/{userId}", null);
            response.EnsureSuccessStatusCode();
        }

        public async Task ToggleEnabledAsync(int userId)
        {
            var response = await _httpClient.PostAsync($"api/user/toggle/{userId}", null);
            response.EnsureSuccessStatusCode();
        }

        public async Task<bool> InitiatePasswordResetAsync(string email)
        {
            var response = await _httpClient.PostAsJsonAsync("api/user/initiate-password-reset", new { Email = email });
            return response.IsSuccessStatusCode;
        }

        public async Task<bool> VerifyResetTokenAsync(string token)
        {
            var response = await _httpClient.GetAsync($"api/user/verify-reset-token?token={token}");
            return response.IsSuccessStatusCode;
        }

        public async Task<bool> ResetPasswordAsync(string token, string newPassword)
        {
            // API'ye ResetPasswordRequest modelini gönderiyoruz
            var response = await _httpClient.PostAsJsonAsync("api/user/reset-password", new
            {
                Token = token,
                NewPassword = newPassword
            });

            return response.IsSuccessStatusCode;
        }


        // --- Mevcut Kullanıcı Metotları (Profil Sayfası İçin) ---
        public async Task<UserDTO?> GetCurrentUserAsync(int userId, bool includeOrders = false, bool includeProducts = false)
        {
            // Zaten halihazırda var olan GetAsync metodunu çağırabiliriz
            return await GetAsync(userId, includeOrders, includeProducts);
        }

        public async Task UpdateCurrentUserAsync(UserDTO dto)
        {
            // Zaten halihazırda var olan UpdateAsync metodunu çağırabiliriz
            await UpdateAsync(dto);
        }

        // --- Parametresiz RequestSellerStatus (Web.Mvc tarafı)
        public async Task RequestSellerStatusAsync()
        {
            //interface gereği buraya boş bir implementasyon veya hata fırlatan bir yapı koyuyoruz.
            throw new NotImplementedException("For Admin operations, this method must be called with a User ID: RequestSellerStatusAsync(int userId)");
        }
    }
    
}
