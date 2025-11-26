using ECommerce.Application.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace ECommerce.Admin.MVC.Controllers
{
    [Route("admin")]
    public class UserController : Controller
    {
        private readonly IUserService _userService;

        public UserController(IUserService userService)
        {
            _userService = userService;
        }

        // GET: /admin/users
        [HttpGet("users")]
        public IActionResult List()
        {
            var users = _userService.GetAll();
            return View(users);
        }

        // GET: /admin/user/5/approve
        [HttpGet("user/{id}/approve")]
        public IActionResult Approve(int id)
        {
            var user = _userService.Get(id);
            return View(user);
        }

        // POST: /admin/user/5/approve
        [HttpPost("user/{id}/approve")]
        [ValidateAntiForgeryToken]
        public IActionResult ApproveConfirmed(int id)
        {
            _userService.ApproveSeller(id);
            return RedirectToAction("List");
        }
    }
}