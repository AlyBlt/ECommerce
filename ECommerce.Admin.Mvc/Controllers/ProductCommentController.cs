using ECommerce.Application.Filters;
using ECommerce.Admin.Mvc.Models.Comment;
using ECommerce.Application.DTOs.ProductComment;
using ECommerce.Application.Interfaces.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ECommerce.Admin.MVC.Controllers
{
    [Route("ProductComment")]
    [Authorize(Policy = "AdminPanelAccess")]
    [ActiveUserAuthorize]
    public class ProductCommentController : Controller
    {
        private readonly IProductCommentService _commentApiService;

        public ProductCommentController(IProductCommentService commentApiService)
        {
            _commentApiService = commentApiService;
        }

        // GET: /ProductComment
        [HttpGet("")]
        public async Task<IActionResult> List()
        {
            // API'den "api/productcomment" GET isteği
            var commentDtos = await _commentApiService.GetAllAsync();

            var model = commentDtos.Select(c => new CommentViewModel
            {
                Id = c.Id,
                UserName = c.UserName,
                ProductName = c.ProductName,
                Text = c.Text,
                Rating = c.StarCount,
                IsApproved = c.IsConfirmed,
                IsRejected = c.IsRejected
            }).ToList();

            return View(model);
        }

        // GET: /ProductComment/ApproveForm/{id}
        [HttpGet("ApproveForm/{id}")]
        public async Task<IActionResult> Approve(int id)
        {
            var dto = await _commentApiService.GetAsync(id);
            if (dto == null)
            {
                TempData["ErrorMessage"] = "Comment not found!";
                return RedirectToAction("List");
            }

            return View(MapDtoToViewModel(dto));
        }

        // POST: /ProductComment/{id}/approve
        [HttpPost("{id}/approve")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ApproveComment(int id)
        {
            // API'ye "api/productcomment/approve/{id}" POST isteği
            await _commentApiService.ApproveCommentAsync(id);
            TempData["SuccessMessage"] = "Comment approved successfully!";
            return RedirectToAction("List");
        }


        // GET: /ProductComment/RejectForm/{id}
        [HttpGet("RejectForm/{id}")]
        public async Task<IActionResult> Reject(int id)
        {
            var dto = await _commentApiService.GetAsync(id);
            if (dto == null)
            {
                TempData["ErrorMessage"] = "Comment not found!";
                return RedirectToAction("List");
            }

            return View(MapDtoToViewModel(dto));
        }

        // POST: /ProductComment/{id}/reject
        [HttpPost("{id}/reject")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> RejectComment(int id)
        {
            // API'ye "api/productcomment/reject/{id}" POST isteği
            await _commentApiService.RejectCommentAsync(id);
            TempData["SuccessMessage"] = "Comment rejected successfully!";
            return RedirectToAction("List");
        }


        //Helper
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