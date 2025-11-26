using ECommerce.Application.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace ECommerce.Admin.MVC.Controllers
{
    //[Route("admin")]
    public class DashboardController : Controller
    {
        private readonly IDashboardService _dashboardService;

        public DashboardController(IDashboardService dashboardService)
        {
            _dashboardService = dashboardService;
        }

        // GET: /admin  (Dashboard)
        //[Route("")]
        //[Route("dashboard")]
        public IActionResult Index()
        {
            var stats = _dashboardService.GetDashboardStats(); 
            return View(stats);
        }

        // GET: /admin/dashboard/error
        //[HttpGet("error")]
        public IActionResult Error()
        {
            return View();  // Hata görünümünü burada gösterebiliriz
        }
    }
}