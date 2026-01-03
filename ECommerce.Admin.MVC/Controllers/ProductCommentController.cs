using ECommerce.Admin.Mvc.Filters;
using ECommerce.Admin.Mvc.Models.Comment;
using ECommerce.Application.DTOs.ProductComment;
using ECommerce.Application.Interfaces.Services;
using ECommerce.Application.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ECommerce.Admin.MVC.Controllers
{
    [Route("ProductComment")]
    [Authorize(Roles = "Admin")]
    [ActiveUserAuthorize]
    public class ProductCommentController : Controller
    {
        private readonly IProductCommentService _commentService;
        
        public ProductCommentController(IProductCommentService commentService)
        {
            _commentService = commentService;
        }

        // GET: /admin/comment
        [HttpGet("")]
        public async Task<IActionResult> List()
        {
            var commentDtos = await _commentService.GetAllAsync();
            // DTO'dan ViewModel'e eşleme (Mapping)
            var model = commentDtos.Select(c => new CommentViewModel
            {
                Id = c.Id,
                UserName = c.UserName, // Serviste birleştirdiğimiz hazır string
                ProductName = c.ProductName,
                Text = c.Text,
                Rating = c.StarCount,
                IsApproved = c.IsConfirmed,
                IsRejected = c.IsRejected
            }).ToList();

            return View(model);
        }

        // GET: /ProductComment/{id}/approve
        [HttpGet("ApproveForm/{id}")]
        public async Task<IActionResult> Approve(int id)
        {
            var dto = await _commentService.GetAsync(id);
            if (dto == null)
            {
                TempData["ErrorMessage"] = "Comment not found!";
                return RedirectToAction("List");
            }

            var model = MapDtoToViewModel(dto);
            return View(model);
        }

        // POST: /admin/comment/5/approve
        [HttpPost("{id}/approve")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ApproveComment(int id)
        {
            var comment = await _commentService.GetAsync(id);
            if (comment == null)
            {
                TempData["ErrorMessage"] = "Comment not found!";
                return RedirectToAction("List");
            }

            await _commentService.ApproveCommentAsync(id);
            TempData["SuccessMessage"] = "Comment approved successfully!";
            return RedirectToAction("List");
        }


        // GET: /ProductComment/RejectForm/{id}
        [HttpGet("RejectForm/{id}")]
        public async Task<IActionResult> Reject(int id)
        {
            var dto = await _commentService.GetAsync(id);
            if (dto == null)
            {
                TempData["ErrorMessage"] = "Comment not found!";
                return RedirectToAction("List");
            }

            var model = MapDtoToViewModel(dto);
            return View(model);
        }


        // POST: /ProductComment/{id}/reject
        [HttpPost("{id}/reject")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> RejectComment(int id)
        {
            var comment = await _commentService.GetAsync(id);
            if (comment == null)
            {
                TempData["ErrorMessage"] = "Comment not found!";
                return RedirectToAction("List");
            }

            await _commentService.RejectCommentAsync(id);
            TempData["SuccessMessage"] = "Comment rejected successfully!";
            return RedirectToAction("List");
        }


        // Kod tekrarını önlemek için yardımcı private metod
        private CommentViewModel MapDtoToViewModel(ProductCommentDTO dto)
        {
            return new CommentViewModel
            {
                Id = dto.Id,
                UserName = dto.UserName,
                ProductName = dto.ProductName,
                Text = dto.Text,
                Rating = dto.StarCount,
                IsApproved = dto.IsConfirmed,
                IsRejected = dto.IsRejected
            };
        }
    }
}