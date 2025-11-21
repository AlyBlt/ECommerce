using Microsoft.AspNetCore.Mvc;

namespace Admin.Mvc.Controllers
{
    public class ProductController : Controller
    {
        public IActionResult Delete(int id)
        {
            ViewBag.Id = id;
            return View();
        }
    }
}
