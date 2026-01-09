using ECommerce.Application.DTOs.Favorite;
using ECommerce.Application.Interfaces.Services;

namespace ECommerce.Web.Mvc.Services
{
    public class FavoriteApiService : IFavoriteService
    {
        private readonly HttpClient _httpClient;

        public FavoriteApiService(IHttpClientFactory httpClientFactory)
        {
            // Program.cs'de tanımladığımız ve Token ekleyen HttpClient
            _httpClient = httpClientFactory.CreateClient("DataApi");
        }

        // Kullanıcının favori ürünlerini getir
        public async Task<IEnumerable<FavoriteDTO>> GetByUserAsync(int userId, string? token = null)
        {
            if (!string.IsNullOrEmpty(token))
                _httpClient.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);

            return await _httpClient.GetFromJsonAsync<IEnumerable<FavoriteDTO>>($"api/favorite/{userId}") ?? new List<FavoriteDTO>();
        }

        // Favorilere ürün ekle
        public async Task AddAsync(int userId, int productId)
        => await _httpClient.PostAsJsonAsync("api/favorite/add", new { userId, productId });

        // Favorilerden ürün çıkar
        public async Task RemoveAsync(int userId, int productId)
        {
            var response = await _httpClient.DeleteAsync($"api/favorite/remove/{userId}/{productId}");
            response.EnsureSuccessStatusCode();
        }

        // Ürün favorilerde mi kontrolü
        public async Task<bool> ExistsAsync(int userId, int productId)
        {
            return await _httpClient.GetFromJsonAsync<bool>($"api/favorite/exists/{userId}/{productId}");
        }

        // Tüm favorileri temizle
        public async Task ClearAsync(int userId)
        {
            var response = await _httpClient.DeleteAsync($"api/favorite/clear/{userId}");
            response.EnsureSuccessStatusCode();
        }

        // Yeni: Session'dakileri topluca gönderir
        public async Task BatchAddAsync(int userId, List<int> productIds, string? token = null)
        {
            if (!string.IsNullOrEmpty(token))
                _httpClient.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);

            var response = await _httpClient.PostAsJsonAsync("api/favorite/batch-add", new { userId, productIds });
            response.EnsureSuccessStatusCode();
        }
    }
}
