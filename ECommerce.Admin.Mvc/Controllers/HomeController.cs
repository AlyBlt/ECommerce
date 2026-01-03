using ECommerce.Admin.Mvc.Filters;
using ECommerce.Admin.Mvc.Models.Home;
using ECommerce.Application.Interfaces.Services;
using ECommerce.Application.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ECommerce.Admin.MVC.Controllers
{
    [Route("admin")]
    [Authorize(Roles = "Admin")]
    [ActiveUserAuthorize]
    public class HomeController : Controller
    {
        private readonly IDashboardService _dashboardService;
       
        public HomeController(IDashboardService dashboardService)
        {
            _dashboardService = dashboardService;
        }

        [HttpGet("")]
        public async Task<IActionResult> Index()
        {
            // Buraya erişiyorsa:
            // Kullanıcı login
            // Role = Admin (Authorize attribute bunu kontrol ediyor)
            if (User.Identity?.IsAuthenticated == false)
            {
                return RedirectToAction("Login", "Auth");
            }

            // Dashboard verilerini al
            var statsDto = await _dashboardService.GetDashboardStatsAsync();

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