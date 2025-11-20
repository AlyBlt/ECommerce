using ECommerceWeb.MVC.Models;
using ECommerceWeb.MVC.Services;
using Microsoft.AspNetCore.Mvc;

namespace ECommerceWeb.MVC.Controllers
{
    public class ProductController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
        public IActionResult GetAllProducts()
        {
            var products = ProductService.GetAllProducts();

            return View("dsfsdfdsf");
        }
    }
}
