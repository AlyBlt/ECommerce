using ECommerce.Application.DTOs.Category;
using ECommerce.Application.Interfaces.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ECommerce.Api.Data.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CategoryController : ControllerBase
    {
        private readonly ICategoryService _categoryService;

        public CategoryController(ICategoryService categoryService)
        {
            _categoryService = categoryService;
        }

        // GET: api/category (Herkes erişebilir)
        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var categories = await _categoryService.GetAllAsync();
            return Ok(categories);
        }

        // GET: api/category/5 (Herkes erişebilir)
        [HttpGet("{id}")]
        public async Task<IActionResult> Get(int id)
        {
            var category = await _categoryService.GetAsync(id);
            if (category == null) return NotFound();
            return Ok(category);
        }

        // POST: api/category (Sadece Admin)
        [Authorize(Policy = "AdminPanelAccess")]
        [HttpPost]
        public async Task<IActionResult> Create([FromBody] CategoryDTO dto)
        {
            await _categoryService.AddAsync(dto);
            return Ok(); // CreatedAtAction da dönebiliriz
        }

        // PUT: api/category (Sadece Admin)
        [Authorize(Policy = "AdminPanelAccess")]
        [HttpPut]
        public async Task<IActionResult> Update([FromBody] CategoryDTO dto)
        {
            try
            {
                await _categoryService.UpdateAsync(dto);
                return NoContent();
            }
            catch (KeyNotFoundException)
            {
                return NotFound();
            }
        }

        // DELETE: api/category/5 (Sadece Admin)
        [Authorize(Policy = "AdminPanelAccess")]
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            try
            {
                await _categoryService.DeleteAsync(id);
                return NoContent();
            }
            catch (InvalidOperationException ex)
            {
                // "Kategoriye bağlı ürünler var" gibi hatalar için
                return BadRequest(new { message = "Cannot delete category: " + ex.Message });
            }
            catch (KeyNotFoundException)
            {
                return NotFound(new { message = "Category not found" });
            }
        }

        // GET: api/category/has-products/5 (Admin Panelindeki kontrol için)
        [HttpGet("has-products/{id}")]
        public async Task<IActionResult> HasProducts(int id)
        {
            var hasProducts = await _categoryService.HasProductsAsync(id);
            return Ok(hasProducts);
        }
    }
}
