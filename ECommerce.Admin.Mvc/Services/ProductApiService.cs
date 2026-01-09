using ECommerce.Application.DTOs.Product;
using ECommerce.Application.DTOs.ProductComment;
using ECommerce.Application.Interfaces.Services;

namespace ECommerce.Admin.Mvc.Services
{
    public class ProductApiService : IProductService
    {
        private readonly HttpClient _httpClient;

        public ProductApiService(IHttpClientFactory httpClientFactory)
        {
            // Program.cs'de tanımladığımız "DataApi" HttpClient'ını alıyoruz
            _httpClient = httpClientFactory.CreateClient("DataApi");
        }

        // --- TEMEL OKUMA (GET) İŞLEMLERİ ---

        public async Task<IEnumerable<ProductDTO>> GetAllAsync()
        {
            return await _httpClient.GetFromJsonAsync<IEnumerable<ProductDTO>>("api/product")
                   ?? new List<ProductDTO>();
        }

        public async Task<ProductDTO?> GetAsync(int id)
        {
            return await _httpClient.GetFromJsonAsync<ProductDTO>($"api/product/{id}");
        }

        // --- ÖZEL LİSTELEME İŞLEMLERİ ---

        public async Task<List<ProductDTO>> GetActiveProductsAsync()
        {
            return await _httpClient.GetFromJsonAsync<List<ProductDTO>>("api/product/active")
                   ?? new List<ProductDTO>();
        }

        public async Task<List<ProductDTO>> GetFeaturedProductsAsync()
        {
            return await _httpClient.GetFromJsonAsync<List<ProductDTO>>("api/product/featured")
                   ?? new List<ProductDTO>();
        }

        public async Task<List<ProductDTO>> GetDiscountedProductsAsync()
        {
            return await _httpClient.GetFromJsonAsync<List<ProductDTO>>("api/product/discounted")
                   ?? new List<ProductDTO>();
        }

        public async Task<List<ProductDTO>> GetNewArrivalProductsAsync()
        {
            return await _httpClient.GetFromJsonAsync<List<ProductDTO>>("api/product/new-arrivals")
                   ?? new List<ProductDTO>();
        }

        public async Task<List<ProductDTO>> GetRelatedProductsAsync(int categoryId, int currentProductId)
        {
            return await _httpClient.GetFromJsonAsync<List<ProductDTO>>($"api/product/related/{categoryId}/{currentProductId}")
                   ?? new List<ProductDTO>();
        }

        // --- ARAMA VE FİLTRELEME ---

        public async Task<List<ProductDTO>> SearchProductsAsync(string query, string? category, byte? rating)
        {
            // Query string oluşturma: api/product/search?query=telefon&category=elektronik...
            var url = $"api/product/search?query={query}&category={category}&rating={rating}";
            return await _httpClient.GetFromJsonAsync<List<ProductDTO>>(url) ?? new List<ProductDTO>();
        }

        // --- YAZMA (POST/PUT/DELETE) İŞLEMLERİ ---

        public async Task AddAsync(ProductDTO dto)
        {
            var response = await _httpClient.PostAsJsonAsync("api/product", dto);
            response.EnsureSuccessStatusCode();
        }

        public async Task UpdateAsync(ProductDTO dto)
        {
            var response = await _httpClient.PutAsJsonAsync("api/product", dto);
            response.EnsureSuccessStatusCode();
        }

        public async Task DeleteAsync(int id)
        {
            var response = await _httpClient.DeleteAsync($"api/product/{id}");
            response.EnsureSuccessStatusCode();
        }

        public async Task ToggleStatusAsync(int id)
        {
            var response = await _httpClient.PostAsync($"api/product/toggle/{id}", null);
            response.EnsureSuccessStatusCode();
        }

        // --- YORUM VE SATIN ALMA KONTROLÜ ---

        public async Task AddCommentAsync(int productId, ProductCommentDTO commentDto)
        {
            var response = await _httpClient.PostAsJsonAsync($"api/product/{productId}/comment", commentDto);
            response.EnsureSuccessStatusCode();
        }

        public async Task<bool> HasUserPurchasedProductAsync(int userId, int productId)
        {
            return await _httpClient.GetFromJsonAsync<bool>($"api/product/check-purchase/{userId}/{productId}");
        }
    }
}
