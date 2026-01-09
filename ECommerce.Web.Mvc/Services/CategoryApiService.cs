using ECommerce.Application.DTOs.Category;
using ECommerce.Application.Interfaces.Services;

namespace ECommerce.Web.Mvc.Services
{
    public class CategoryApiService : ICategoryService
    {
        private readonly HttpClient _httpClient;

        public CategoryApiService(IHttpClientFactory httpClientFactory)
        {
            // Program.cs'deki "DataApi" yapılandırmasını kullanıyoruz
            _httpClient = httpClientFactory.CreateClient("DataApi");
        }

        // Tüm kategorileri API'den çek
        public async Task<IEnumerable<CategoryDTO>> GetAllAsync()
        {
            return await _httpClient.GetFromJsonAsync<IEnumerable<CategoryDTO>>("api/category")
                   ?? new List<CategoryDTO>();
        }

        // Tek bir kategoriyi ID ile API'den çek
        public async Task<CategoryDTO?> GetAsync(int id)
        {
            return await _httpClient.GetFromJsonAsync<CategoryDTO>($"api/category/{id}");
        }

        // Yeni kategori ekleme isteğini API'ye gönder
        public async Task AddAsync(CategoryDTO catDto)
        {
            var response = await _httpClient.PostAsJsonAsync("api/category", catDto);
            response.EnsureSuccessStatusCode();
        }

        // Kategori güncelleme isteğini API'ye gönder
        public async Task UpdateAsync(CategoryDTO catDto)
        {
            var response = await _httpClient.PutAsJsonAsync("api/category", catDto);
            response.EnsureSuccessStatusCode();
        }

        // Kategori silme isteğini API'ye gönder
        public async Task DeleteAsync(int id)
        {
            var response = await _httpClient.DeleteAsync($"api/category/{id}");
            response.EnsureSuccessStatusCode();
        }

        // Bu kategoride ürün var mı kontrolünü API'ye sor
        public async Task<bool> HasProductsAsync(int categoryId)
        {
            return await _httpClient.GetFromJsonAsync<bool>($"api/category/has-products/{categoryId}");
        }
    }
}
