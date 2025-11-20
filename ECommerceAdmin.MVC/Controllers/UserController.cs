using Microsoft.AspNetCore.Mvc;

namespace Admin.Mvc.Controllers
{
    public class UserController : Controller
    {
        // Kullanıcı listeleme
        public IActionResult List()
        {
            return View();
        }

        // Satıcı olma onayı
        public IActionResult Approve(int id)
        {
            ViewBag.Id = id;
            return View();
        }
    }
}
