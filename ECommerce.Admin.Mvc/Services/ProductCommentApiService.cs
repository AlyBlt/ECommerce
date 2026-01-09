using ECommerce.Application.DTOs.ProductComment;
using ECommerce.Application.Interfaces.Services;

namespace ECommerce.Admin.Mvc.Services
{
    public class ProductCommentApiService : IProductCommentService
    {
        private readonly HttpClient _httpClient;

        public ProductCommentApiService(IHttpClientFactory httpClientFactory)
        {
            _httpClient = httpClientFactory.CreateClient("DataApi");
        }

        public async Task<IEnumerable<ProductCommentDTO>> GetAllAsync()
        {
            return await _httpClient.GetFromJsonAsync<IEnumerable<ProductCommentDTO>>("api/productcomment")
                   ?? new List<ProductCommentDTO>();
        }

        public async Task<ProductCommentDTO?> GetAsync(int id)
        {
            return await _httpClient.GetFromJsonAsync<ProductCommentDTO>($"api/productcomment/{id}");
        }

        public async Task AddAsync(ProductCommentDTO dto)
        {
            var response = await _httpClient.PostAsJsonAsync("api/productcomment", dto);
            response.EnsureSuccessStatusCode();
        }

        public async Task UpdateAsync(ProductCommentDTO dto)
        {
            var response = await _httpClient.PutAsJsonAsync($"api/productcomment/{dto.Id}", dto);
            response.EnsureSuccessStatusCode();
        }

        public async Task DeleteAsync(int id)
        {
            var response = await _httpClient.DeleteAsync($"api/productcomment/{id}");
            response.EnsureSuccessStatusCode();
        }

        // Admin Tarafı: Yorum Onaylama
        public async Task ApproveCommentAsync(int id)
        {
            var response = await _httpClient.PostAsync($"api/productcomment/approve/{id}", null);
            response.EnsureSuccessStatusCode();
        }

        // Admin Tarafı: Yorum Reddetme
        public async Task RejectCommentAsync(int id)
        {
            var response = await _httpClient.PostAsync($"api/productcomment/reject/{id}", null);
            response.EnsureSuccessStatusCode();
        }
    }
}
