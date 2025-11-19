using System.Diagnostics;
using ECommerce.Web.MVC.Models;
using Microsoft.AspNetCore.Mvc;

namespace ECommerce.Web.MVC.Controllers
{             //AliyeNot: Routelar düzenlenecek.
              //ProductDetail action’ý parametre almalý:Ödevde örnek route var: /product/{categoryName}-{title}-{id}/details
              //Þu an action parametresiz, ileride Product ID veya kategori bilgisi ile çalýþacaksa eklenmeli
              //yani-->>public IActionResult ProductDetail(int id, string categoryName, string title)
    public class HomeController : Controller
    {

        //[Route("home")]-->>böyle olabilir-->>URL’yi SEO ve okunabilirlik açýsýndan ayarlýyor.
        public IActionResult Index()
        {
            return View();
        }

        public IActionResult AboutUs()
        {
            return View(); 
        }
        [Route("contact")] //Contact->contact yapýldý c küçük harfle--SEO dostu
        public IActionResult Contact()
        {
            return View();
        }

        public IActionResult Listing()
        {
            return View();
        }

        //[Route("product/{id}")] --->> public IActionResult ProductDetail(int id)
        public IActionResult ProductDetail()
        {
            return View();
        }
    }
}
