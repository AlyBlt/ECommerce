using ECommerce.Application.DTOs.Dashboard;
using ECommerce.Application.Interfaces.Repositories;
using ECommerce.Application.Interfaces.Services;


public class DashboardService : IDashboardService
{
    private readonly IDashboardRepository _dashboardRepository;

    public DashboardService(IDashboardRepository dashboardRepository)
    {
        _dashboardRepository = dashboardRepository;
    }

    public async Task<DashboardStatsDTO> GetDashboardStatsAsync()
    {
        return new DashboardStatsDTO
        {
            TotalUsers = await _dashboardRepository.GetTotalUsersAsync(),
            TotalProducts = await _dashboardRepository.GetTotalProductsAsync(),
            TotalCategories = await _dashboardRepository.GetTotalCategoriesAsync(),
            PendingComments = await _dashboardRepository.GetPendingCommentsAsync(),
            PendingSellers = await _dashboardRepository.GetPendingSellersAsync(),
            DailySales = 0 // Eğer ileride hesaplanacaksa buradan set edilir
        };
    }
}