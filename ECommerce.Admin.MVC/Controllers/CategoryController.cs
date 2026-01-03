using ECommerce.Admin.Mvc.Filters;
using ECommerce.Admin.Mvc.Models.Category;
using ECommerce.Application.DTOs.Category;
using ECommerce.Application.Interfaces.Services;
using ECommerce.Application.Services;
using ECommerce.Domain.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Threading.Tasks;

namespace ECommerce.Admin.MVC.Controllers
{
    [Route("admin/category")]
    [Authorize(Roles = "Admin")]
    [ActiveUserAuthorize]
    public class CategoryController : Controller
    {
        private readonly ICategoryService _categoryService;
        
        public CategoryController(ICategoryService categoryService)
        {
            _categoryService = categoryService;
        }


        // /admin/category  -- DTO -> ListViewModel Mapping
        [HttpGet("")]
        public async Task<IActionResult> List()
        {
            var dtos = await _categoryService.GetAllAsync();
            var list = dtos.Select(c => new CategoryListViewModel
            {
                Id = c.Id,
                Name = c.Name,
                Color = c.Color,
                IconCssClass = c.IconCssClass
            }).ToList();

            return View(list);
        }


        // /admin/category/create
        [HttpGet("create")]
        public IActionResult Create()
        {
           return View();
        }

        // CREATE POST
        [HttpPost("create")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(CategoryCreateEditViewModel model)
        {
            if (!ModelState.IsValid)
                return View(model);

            // ViewModel'den DTO'ya Manuel Mapping
            var dto = new CategoryDTO
            {
                Name = model.Name,
                Color = model.Color,
                IconCssClass = model.IconCssClass
            };

            await _categoryService.AddAsync(dto);
            TempData["Success"] = "Category created successfully!";
            return RedirectToAction(nameof(List));
        }

        // Edit GET (Formu görüntüle) -- DTO -> CreateEditViewModel Mapping
        [HttpGet("{id}/edit")]
        public async Task<IActionResult> Edit(int id)
        {
            var dto = await _categoryService.GetAsync(id);
            if (dto == null)
            {
                TempData["Error"] = "Category not found!";
                return RedirectToAction(nameof(List));
            }

            var vm = new CategoryCreateEditViewModel
            {
                Id = dto.Id,
                Name = dto.Name,
                Color = dto.Color,
                IconCssClass = dto.IconCssClass
            };

            return View(vm);
        }

        // Edit POST (Veriyi kaydetme işlemi) --- CreateEditViewModel -> DTO Mapping
        [HttpPost("{id}/edit")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(CategoryCreateEditViewModel model)
        {
            if (!ModelState.IsValid)
                return View(model);

            var dto = new CategoryDTO
            {
                Id = model.Id,
                Name = model.Name,
                Color = model.Color,
                IconCssClass = model.IconCssClass
            };

            try
            {
                await _categoryService.UpdateAsync(dto);
                TempData["Success"] = "Category updated successfully!";
            }
            catch (KeyNotFoundException)
            {
                TempData["Error"] = "Category not found!";
            }

            return RedirectToAction(nameof(List));
           
        }


        [HttpGet("{id}/delete")]
        public async Task<IActionResult> Delete(int id)
        {
            var dto = await _categoryService.GetAsync(id);
            if (dto == null)
            {
                TempData["Error"] = "Category not found!";
                return RedirectToAction(nameof(List));
            }

            var vm = new CategoryCreateEditViewModel
            {
                Id = dto.Id,
                Name = dto.Name,
                Color = dto.Color,
                IconCssClass = dto.IconCssClass
            };

            return View(vm);
        }


        // DELETE POST
        [HttpPost("delete-confirmed")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            try
            {
                var category = await _categoryService.GetAsync(id);
                if (category == null)
                {
                    TempData["Error"] = "Category not found!";
                    return RedirectToAction("List");
                }

                // Kategoriye bağlı ürün varsa, silme işlemini engelle
                bool hasProducts = await _categoryService.HasProductsAsync(id);
                if (hasProducts)
                {
                    TempData["Error"] = "Cannot delete category with products. Remove or reassign products first.";
                    return RedirectToAction("List");
                }

                await _categoryService.DeleteAsync(id);  // Asenkron silme işlemi
                TempData["Success"] = "Category deleted successfully!";
                return RedirectToAction("List");
            }
            catch (InvalidOperationException ex)
            {
                TempData["Error"] = ex.Message;
                return RedirectToAction("List");
            }
        }
    }

}
