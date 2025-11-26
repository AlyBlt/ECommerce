using ECommerce.Application.Interfaces;
using ECommerce.Application.ViewModels;
using ECommerce.Data.Entities;
using Microsoft.AspNetCore.Mvc;

namespace ECommerce.Admin.MVC.Controllers
{
    [Route("admin/comment")]
    public class ProductCommentController : Controller
    {
        private readonly IProductCommentService _commentService;

        public ProductCommentController(IProductCommentService commentService)
        {
            _commentService = commentService;
        }

        // GET: /admin/comment
        [HttpGet("")]
        public IActionResult List()
        {
            var comments = _commentService.GetAll()
                .Select(c => new CommentViewModel
                {
                    Id = c.Id,
                    UserName = c.User?.FirstName + " " + c.User?.LastName,
                    ProductName = c.Product?.Name ?? "",
                    Text = c.Text,
                    Rating = c.StarCount,
                    IsApproved = c.IsConfirmed
                })
                .ToList();

            return View(comments);
        }

        // GET: /admin/comment/5/approve
        [HttpGet("{id}/approve")]
        public IActionResult ApproveForm(int id)
        {
            var c = _commentService.Get(id);
            if (c == null)
            {
                TempData["ErrorMessage"] = "Comment not found!";
                return RedirectToAction("List");
            }

            var model = new CommentViewModel
            {
                Id = c.Id,
                UserName = c.User?.FirstName + " " + c.User?.LastName,
                ProductName = c.Product?.Name ?? "",
                Text = c.Text,
                Rating = c.StarCount,
                IsApproved = c.IsConfirmed
            };

            return View(model);
        }

        // POST: /admin/comment/5/approve
        [HttpPost("{id}/approve")]
        [ValidateAntiForgeryToken]
        public IActionResult ApproveComment(int id)  
        {
            var comment = _commentService.Get(id);
            if (comment == null)
            {
                TempData["ErrorMessage"] = "Comment not found!";
                return RedirectToAction("List");
            }

            _commentService.ApproveComment(id); 
            TempData["SuccessMessage"] = "Comment approved successfully!";
            return RedirectToAction("List");
        }
    }
}