using ECommerce.Admin.Mvc.Models.Category;
using ECommerce.Admin.Mvc.Services;
using ECommerce.Application.DTOs.Category;
using ECommerce.Application.Filters;
using ECommerce.Application.Interfaces.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;


namespace ECommerce.Admin.MVC.Controllers
{
    [Route("admin/category")]
    [Authorize(Policy = "AdminPanelAccess")]
    [ActiveUserAuthorize]
    public class CategoryController : Controller
    {
        private readonly ICategoryService _categoryApiService;
        private readonly FileApiService _fileApiService;

        public CategoryController(ICategoryService categoryApiService, FileApiService fileApiService)
        {
            _categoryApiService = categoryApiService;
            _fileApiService = fileApiService;
        }


        // GET: /admin/category
        [HttpGet("")]
        public async Task<IActionResult> List()
        {
            // API'den "api/category" endpoint'ine GET isteği atar
            var dtos = await _categoryApiService.GetAllAsync();
            var list = dtos.Select(c => new CategoryListViewModel
            {
                Id = c.Id,
                Name = c.Name,
                Color = c.Color,
                IconCssClass = c.IconCssClass,
                ImageUrl = c.ImageUrl
            }).ToList();

            return View(list);
        }


        // /admin/category/create
        [HttpGet("create")]
        public IActionResult Create()
        {
           return View();
        }

       
        [HttpPost("create")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(CategoryCreateEditViewModel model)
        {
            // Dosya Yükleme Mantığı
            string? uploadedFileName = null;
            if (model.ImageFile != null && model.ImageFile.Length > 0)
            {
                uploadedFileName = await _fileApiService.UploadFileAsync(model.ImageFile);
            }

            ModelState.Remove("ImageUrl"); // Manuel atayacağımız için validasyondan çıkar

            if (!ModelState.IsValid) return View(model);

            var dto = new CategoryDTO
            {
                Name = model.Name,
                Color = model.Color,
                IconCssClass = model.IconCssClass,
                ImageUrl = uploadedFileName // Yüklenen dosya adını veriyoruz
            };

            // API'ye "api/category" üzerinden POST isteği atar
            await _categoryApiService.AddAsync(dto);
            TempData["Success"] = "Category created successfully!";
            return RedirectToAction(nameof(List));
        }

        [HttpGet("{id}/edit")]
        public async Task<IActionResult> Edit(int id)
        {
            var dto = await _categoryApiService.GetAsync(id);
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
                IconCssClass = dto.IconCssClass,
                ImageUrl = dto.ImageUrl
            };

            return View(vm);
        }

        [HttpPost("{id}/edit")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(CategoryCreateEditViewModel model)
        {
            // Mevcut kategoriyi getir
            var existingDto = await _categoryApiService.GetAsync(model.Id);
            if (existingDto == null) return NotFound();

            if (!ModelState.IsValid) return View(model);

            // Yeni resim seçilmiş mi kontrolü
            string? finalImageUrl = model.ImageUrl; // Varsayılan olarak eskisi kalsın
            if (model.ImageFile != null && model.ImageFile.Length > 0)
            {
                finalImageUrl = await _fileApiService.UploadFileAsync(model.ImageFile);
            }

            var dto = new CategoryDTO
            {
                Id = model.Id,
                Name = model.Name,
                Color = model.Color,
                IconCssClass = model.IconCssClass,
                ImageUrl = finalImageUrl
            };

            // API'ye "api/category" üzerinden PUT isteği atar
            await _categoryApiService.UpdateAsync(dto);
            TempData["Success"] = "Category updated successfully!";
            return RedirectToAction(nameof(List));
        }

        [HttpGet("{id}/delete")]
        public async Task<IActionResult> Delete(int id)
        {
            var dto = await _categoryApiService.GetAsync(id);
            if (dto == null)
            {
                TempData["Error"] = "Category not found!";
                return RedirectToAction(nameof(List));
            }

            return View(new CategoryCreateEditViewModel
            {
                Id = dto.Id,
                Name = dto.Name,
                Color = dto.Color,
                IconCssClass = dto.IconCssClass
            });
        }

        [HttpPost("delete-confirmed")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            // Önce API'ye bu kategoride ürün var mı diye soruyoruz
            bool hasProducts = await _categoryApiService.HasProductsAsync(id);
            if (hasProducts)
            {
                TempData["Error"] = "Cannot delete category with products!";
                return RedirectToAction("List");
            }

            // API'ye "api/category/{id}" üzerinden DELETE isteği atar
            await _categoryApiService.DeleteAsync(id);
            TempData["Success"] = "Category deleted successfully!";
            return RedirectToAction("List");
        }
    }

}
