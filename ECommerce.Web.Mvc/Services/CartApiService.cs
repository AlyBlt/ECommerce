using ECommerce.Application.DTOs.Cart;
using ECommerce.Application.Interfaces.Services;

namespace ECommerce.Web.Mvc.Services
{
    public class CartApiService : ICartService
    {
        private readonly HttpClient _httpClient;

        public CartApiService(IHttpClientFactory httpClientFactory)
        {
            // Program.cs'de yapılandırdığımız, Token taşıyan "DataApi" istemcisi
            _httpClient = httpClientFactory.CreateClient("DataApi");
        }

        // Sepetteki ürünleri getir
        public async Task<IEnumerable<CartItemDTO>> GetCartItemsAsync(int userId, string? token = null)
        {
            if (!string.IsNullOrEmpty(token))
            {
                _httpClient.DefaultRequestHeaders.Authorization =
                    new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);
            }

            return await _httpClient.GetFromJsonAsync<IEnumerable<CartItemDTO>>($"api/cart/{userId}")
                   ?? new List<CartItemDTO>();
        }

        // Sepete ürün ekle (CartAddDTO kullanarak)
        public async Task AddToCartAsync(int userId, CartAddDTO dto)
        {
            // API tarafında userId'yi URL'den veya Token'dan alabiliriz
            // Biz burada senin metod imzandaki userId'yi URL'ye ekliyoruz
            var response = await _httpClient.PostAsJsonAsync($"api/cart/add/{userId}", dto);
            response.EnsureSuccessStatusCode();
        }

        // Sepetteki bir ürünün miktarını güncelle
        public async Task UpdateCartItemAsync(int userId, int productId, int quantity)
        {
            // Parametreleri API'ye QueryString veya Body olarak gönderebiliriz.Body daha sağlıklıdır.
            var response = await _httpClient.PutAsJsonAsync("api/cart/update-item", new { userId, productId, quantity });
            response.EnsureSuccessStatusCode();
        }

        // Sepetten tek bir ürünü kaldır
        public async Task RemoveCartItemAsync(int userId, int productId)
        {
            var response = await _httpClient.DeleteAsync($"api/cart/remove/{userId}/{productId}");
            response.EnsureSuccessStatusCode();
        }

        // Kullanıcının tüm sepetini temizle
        public async Task ClearCartAsync(int userId)
        {
            var response = await _httpClient.DeleteAsync($"api/cart/clear/{userId}");
            response.EnsureSuccessStatusCode();

        }

        // Sepeti senkronize et (Login sonrası Session'dan gelenler)
        public async Task SyncCartAsync(int userId, List<CartAddDTO> items, string? token = null)
        {
            if (!string.IsNullOrEmpty(token))
            {
                _httpClient.DefaultRequestHeaders.Authorization =
                    new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);
            }

            var response = await _httpClient.PostAsJsonAsync($"api/cart/sync/{userId}", items);
            response.EnsureSuccessStatusCode();
        }
    }
}
