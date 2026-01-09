using ECommerce.Application.Filters;
using ECommerce.Admin.Mvc.Models.Product;
using ECommerce.Application.Interfaces.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ECommerce.Admin.MVC.Controllers
{
    [Route("admin/products")]
    [Authorize(Policy = "AdminPanelAccess")]
    [ActiveUserAuthorize]
    public class ProductController : Controller
    {
        private readonly IProductService _productApiService;

        public ProductController(IProductService productApiService)
        {
            _productApiService = productApiService;
        }

        // GET: /admin/products
        [HttpGet("")]
        public async Task<IActionResult> List()
        {
            // API'den api/product üzerinden tüm ürünleri çekiyoruz
            var productDtos = await _productApiService.GetAllAsync();

            var vm = productDtos.Select(p => new ProductListViewModel
            {
                Id = p.Id,
                Name = p.Name,
                Price = p.Price,
                CategoryName = p.CategoryName ?? "No Category",
                Enabled = p.Enabled,
                SellerName = p.SellerName,
                MainImageUrl = p.MainImageUrl
            }).ToList();

            return View(vm);
        }

        // POST: /admin/products/toggle
        [HttpPost("toggle")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ToggleStatus(int id)
        {
            // API'deki api/product/toggle/{id} endpointine POST isteği atar
            await _productApiService.ToggleStatusAsync(id);
            TempData["Success"] = "Product status updated.";
            return RedirectToAction("List");
        }


        // GET: /admin/products/{id}/delete
        [HttpGet("{id}/delete")]
        public async Task<IActionResult> Delete(int id)
        {
            // API'den api/product/{id} üzerinden ürün detayını alıyoruz
            var dto = await _productApiService.GetAsync(id);
            if (dto == null) return NotFound();

            var model = new ProductDeleteViewModel
            {
                Id = dto.Id,
                Name = dto.Name,
                CategoryName = dto.CategoryName ?? "N/A",
                Price = dto.Price
            };

            return View(model);
        }

        // POST: /admin/products/{id}/delete
        [HttpPost("{id}/delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            try
            {
                // API'ye api/product/{id} üzerinden DELETE isteği atar
                await _productApiService.DeleteAsync(id);
                TempData["Success"] = "Product deleted successfully!";
            }
            catch (Exception ex)
            {
                TempData["Error"] = $"Error: {ex.Message}";
            }

            return RedirectToAction("List");
        }
    }
}


