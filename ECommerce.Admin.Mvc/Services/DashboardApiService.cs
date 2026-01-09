using ECommerce.Application.DTOs.Dashboard;
using ECommerce.Application.Interfaces.Services;

namespace ECommerce.Admin.Mvc.Services
{
    public class DashboardApiService : IDashboardService
    {
        private readonly HttpClient _httpClient;

        public DashboardApiService(IHttpClientFactory httpClientFactory)
        {
            // Admin paneli olduğu için mutlaka Token taşıyan "DataApi" istemcisini kullanıyoruz
            _httpClient = httpClientFactory.CreateClient("DataApi");
        }

        public async Task<DashboardStatsDTO> GetDashboardStatsAsync()
        {
            // API'den istatistikleri çekiyoruz
            return await _httpClient.GetFromJsonAsync<DashboardStatsDTO>("api/dashboard/stats")
                   ?? new DashboardStatsDTO();
        }
    }
}
