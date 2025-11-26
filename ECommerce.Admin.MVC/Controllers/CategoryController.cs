using ECommerce.Application.Interfaces;
using ECommerce.Data.Entities;
using Microsoft.AspNetCore.Mvc;

namespace ECommerce.Admin.MVC.Controllers
{
    [Route("admin/category")]
    public class CategoryController : Controller
    {
        private readonly ICategoryService _categoryService;

        public CategoryController(ICategoryService categoryService)
        {
            _categoryService = categoryService;
        }

        // /admin/category
        [HttpGet("")]
        public IActionResult List()
        {
            var list = _categoryService.GetAll();
            return View(list);
        }


        // /admin/category/create
        [HttpGet("create")]
        public IActionResult Create()
        {
            return View();
        }


        // POST: /admin/category/create
        [HttpPost("create")]
        [ValidateAntiForgeryToken]
        public IActionResult Create(CategoryEntity model)
        {
            _categoryService.Add(model);
            return RedirectToAction("List");
        }

        // /admin/category/5/edit
        [HttpGet("{id}/edit")]
        public IActionResult Edit(int id)
        {
            var model = _categoryService.Get(id);
            if (model == null)
            {
                TempData["ErrorMessage"] = "Category not found!";
                return RedirectToAction("List");
            }
            return View(model);
        }

        // POST: /admin/category/edit
        [HttpPost("edit")]
        [ValidateAntiForgeryToken]
        public IActionResult Edit(CategoryEntity model)
        {
            _categoryService.Update(model);
            return RedirectToAction("List");
        }

        // /admin/category/5/delete
        [HttpGet("{id}/delete")]
        public IActionResult Delete(int id)
        {
            var model = _categoryService.Get(id);
            if (model == null)
            {
                TempData["ErrorMessage"] = "Category not found!";
                return RedirectToAction("List");
            }
            return View(model);
        }

        // POST: /admin/category/delete-confirmed
        [HttpPost("delete-confirmed")]
        [ValidateAntiForgeryToken]
        public IActionResult DeleteConfirmed(int id)
        {
            _categoryService.Delete(id);
            return RedirectToAction("List");
        }

      

    }
}