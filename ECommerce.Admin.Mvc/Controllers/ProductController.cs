using ECommerce.Admin.Mvc.Filters;
using ECommerce.Admin.Mvc.Models.Product;
using ECommerce.Application.Interfaces.Services;
using ECommerce.Application.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ECommerce.Admin.MVC.Controllers
{
    [Route("admin/products")]
    [Authorize(Roles = "Admin")]
    [ActiveUserAuthorize]
    public class ProductController : Controller
    {
        private readonly IProductService _productService;
        
        public ProductController(IProductService productService)
        {
            _productService = productService;
           
        }

        // GET: /admin/products
        [HttpGet("")]
        public async Task<IActionResult> List()
        {

            // Servis artık List<ProductDTO> dönüyor
            var productDtos = await _productService.GetAllAsync();

            // DTO -> Admin List ViewModel Mapping
            var vm = productDtos.Select(p => new ProductListViewModel
            {
                Id = p.Id,
                Name = p.Name,
                Price = p.Price,
                CategoryName = p.CategoryName ?? "No Category",
                Enabled = p.Enabled,
                SellerName = p.SellerName // Admin kimin sattığını da görmeli--view da ekle
            }).ToList();

            return View(vm);
        }



        // POST: /admin/products/toggle
        [HttpPost("toggle")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ToggleStatus(int id)
        {
            await _productService.ToggleStatusAsync(id);
            TempData["Success"] = "Product status updated.";
            return RedirectToAction("List");
        }



        // GET: /admin/products/{id}/delete
        [HttpGet("{id}/delete")]
        public async Task<IActionResult> Delete(int id)
        {

            var dto = await _productService.GetAsync(id);
            if (dto == null) return NotFound();

            // DTO -> Delete ViewModel Mapping
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
                await _productService.DeleteAsync(id);
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