using System.Diagnostics;
using ECommerce.Web.MVC.Models;
using Microsoft.AspNetCore.Mvc;

namespace ECommerce.Web.MVC.Controllers
{
    public class HomeController : Controller
    {
       
        public IActionResult Index()
        {
            return View();
        }

        public IActionResult AboutUs()
        {
            return View(); 
        }
        [Route("Contact")]
        public IActionResult Contact()
        {
            return View();
        }

        public IActionResult Listing()
        {
            return View();
        }

        public IActionResult ProductDetail()
        {
            return View();
        }
    }
}
