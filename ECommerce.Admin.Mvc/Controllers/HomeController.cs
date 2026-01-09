using ECommerce.Application.Filters;
using ECommerce.Admin.Mvc.Models.Home;
using ECommerce.Application.Interfaces.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ECommerce.Admin.MVC.Controllers
{
    [Route("admin")]
    [Authorize(Policy = "AdminPanelAccess")]
    [ActiveUserAuthorize]
    public class HomeController : Controller
    {
        // Artik doğrudan DashboardApiService somut sınıfını enjekte ediyoruz
        private readonly IDashboardService _dashboardApiService;

        public HomeController(IDashboardService dashboardApiService)
        {
            _dashboardApiService = dashboardApiService;
        }

        [HttpGet("")]
        public async Task<IActionResult> Index()
        {
            // Authenticated kontrolü zaten [Authorize] ile yapılıyor, 
            // ama ekstra güvenlik için kalabilir-istersek silinebilir.
            if (User.Identity?.IsAuthenticated == false)
            {
                return RedirectToAction("Login", "Auth");
            }

            // DashboardApiService üzerinden API'deki "api/dashboard/stats" endpointine gider
            var statsDto = await _dashboardApiService.GetDashboardStatsAsync();

            // DTO -> ViewModel Mapping
            var vm = new DashboardViewModel
            {
                TotalUsers = statsDto.TotalUsers,
                TotalProducts = statsDto.TotalProducts,
                TotalCategories = statsDto.TotalCategories,
                PendingComments = statsDto.PendingComments,
                PendingSellers = statsDto.PendingSellers,
                DailySales = statsDto.DailySales
            };

            return View(vm);
        }
    }
}