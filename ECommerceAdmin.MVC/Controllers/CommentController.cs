using Microsoft.AspNetCore.Mvc;

namespace Admin.Mvc.Controllers
{
    public class CommentController : Controller
    {
        // Yorum listeleme
        public IActionResult List()
        {
            return View();
        }

        // Yorum onaylama
        public IActionResult Approve(int id)
        {
            ViewBag.Id = id;
            return View();
        }
    }
}
