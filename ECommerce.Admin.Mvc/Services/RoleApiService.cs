using ECommerce.Application.DTOs.Role;
using ECommerce.Application.Interfaces.Services;

namespace ECommerce.Admin.Mvc.Services
{
    public class RoleApiService : IRoleService
    {
        private readonly HttpClient _httpClient;

        public RoleApiService(IHttpClientFactory httpClientFactory)
        {
            _httpClient = httpClientFactory.CreateClient("DataApi");
        }

        public async Task<IEnumerable<RoleDTO>> GetAllAsync()
        {
            return await _httpClient.GetFromJsonAsync<IEnumerable<RoleDTO>>("api/role")
                   ?? new List<RoleDTO>();
        }

        public async Task<RoleDTO?> GetAsync(int id)
        {
            return await _httpClient.GetFromJsonAsync<RoleDTO>($"api/role/{id}");
        }

        public async Task AddAsync(RoleDTO roleDto)
        {
            var response = await _httpClient.PostAsJsonAsync("api/role", roleDto);
            response.EnsureSuccessStatusCode();
        }

        public async Task UpdateAsync(RoleDTO roleDto)
        {
            var response = await _httpClient.PutAsJsonAsync($"api/role/{roleDto.Id}", roleDto);
            response.EnsureSuccessStatusCode();
        }

        public async Task DeleteAsync(int id)
        {
            var response = await _httpClient.DeleteAsync($"api/role/{id}");
            response.EnsureSuccessStatusCode();
        }
    }
}
