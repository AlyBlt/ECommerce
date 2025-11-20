using System.Diagnostics;
using ECommerce.Web.MVC.Models;
using Microsoft.AspNetCore.Mvc;

namespace ECommerce.Web.MVC.Controllers
{             
    public class HomeController : Controller
    {
        [Route("/")] // home yazýlmadan sadece site adý ile açýlsýn diye yapýldý.
        [Route("home")] // .../home diyerek sayfaya yönlendirilmesi için yapýldý.
        public IActionResult Index()
        {
            return View();
        }

        [Route("about-us")]
        public IActionResult AboutUs()
        {
            return View(); 
        }

        [Route("contact")]
        public IActionResult Contact()
        {
            return View();
        }

        [Route("listing")]
        public IActionResult Listing()
        {
            return View();
        }

        // Pages altýnda ki shop details sayfasýna yönlendiriliyor.
        //[Route("product/{categoryName}-{title}-{id}/details")]  Veritabaný baðlandýktan sonra route açýlýcak test sürecinde sayfa yok olarak gösteriyor.
        public IActionResult ProductDetail([FromRoute] string categoryName, [FromRoute] string title, [FromRoute] int id)
        {
            return View();
        }
    }
}
