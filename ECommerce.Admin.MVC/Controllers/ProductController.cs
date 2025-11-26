using ECommerce.Application.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace ECommerce.Admin.MVC.Controllers
{
    [Route("admin")]
    public class ProductController : Controller
    {
        private readonly IProductService _productService;

        public ProductController(IProductService productService)
        {
            _productService = productService;
        }

        // GET: /admin/products
        [HttpGet("products")]
        public IActionResult List()
        {
            var products = _productService.GetAll();
            return View(products);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult ToggleStatus(int id)
        {
            _productService.ToggleStatus(id);
            return RedirectToAction("List");
        }
        

        // GET: /admin/product/5/delete
        [HttpGet("product/{id}/delete")]
        public IActionResult Delete(int id)
        {
            var product = _productService.Get(id);
            return View(product);
        }

        // POST: /admin/product/5/delete
        [HttpPost("product/{id}/delete")]
        [ValidateAntiForgeryToken]
        public IActionResult DeleteConfirmed(int id)
        {
            _productService.Delete(id);
            return RedirectToAction("List");
        }
    }
}