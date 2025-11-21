using Microsoft.AspNetCore.Mvc;

namespace Admin.Mvc.Controllers
{
    public class CategoryController : Controller
    {
        // Kategori oluşturma
        public IActionResult Create()
        {
            return View();
        }

        // Kategori düzenleme
        public IActionResult Edit(int id)
        {
            ViewBag.Id = id;
            return View();
        }

        // Kategori silme
        public IActionResult Delete(int id)
        {
            ViewBag.Id = id;
           
            return View();
        }
    }
}
