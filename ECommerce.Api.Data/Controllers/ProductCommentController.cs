using ECommerce.Application.DTOs.ProductComment;
using ECommerce.Application.Interfaces.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ECommerce.Api.Data.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProductCommentController : ControllerBase
    {
        private readonly IProductCommentService _commentService;

        public ProductCommentController(IProductCommentService commentService)
        {
            _commentService = commentService;
        }

        // GET: api/productcomment
        [HttpGet]
        [Authorize(Policy = "AdminPanelAccess")]
        public async Task<IActionResult> GetAll()
        {
            var comments = await _commentService.GetAllAsync();
            return Ok(comments);
        }

        // GET: api/productcomment/5
        [HttpGet("{id}")]
        public async Task<IActionResult> Get(int id)
        {
            var comment = await _commentService.GetAsync(id);
            if (comment == null) return NotFound();
            return Ok(comment);
        }

        // POST: api/productcomment (Genellikle Buyer tarafı kullanır)
        [HttpPost]
        [Authorize]
        public async Task<IActionResult> Create([FromBody] ProductCommentDTO dto)
        {
            await _commentService.AddAsync(dto);
            return Ok();
        }

        // PUT: api/productcomment/5
        [HttpPut("{id}")]
        [Authorize(Policy = "AdminPanelAccess")]
        public async Task<IActionResult> Update(int id, [FromBody] ProductCommentDTO dto)
        {
            dto.Id = id;
            await _commentService.UpdateAsync(dto);
            return NoContent();
        }

        // DELETE: api/productcomment/5
        [HttpDelete("{id}")]
        [Authorize(Policy = "AdminPanelAccess")]
        public async Task<IActionResult> Delete(int id)
        {
            await _commentService.DeleteAsync(id);
            return NoContent();
        }

        // --- ADMIN ONAY/RED İŞLEMLERİ ---

        // POST: api/productcomment/approve/5
        [HttpPost("approve/{id}")]
        [Authorize(Policy = "AdminPanelAccess")]
        public async Task<IActionResult> Approve(int id)
        {
            var comment = await _commentService.GetAsync(id);
            if (comment == null) return NotFound();

            await _commentService.ApproveCommentAsync(id);
            return Ok(new { message = "Comment approved." });
        }

        // POST: api/productcomment/reject/5
        [HttpPost("reject/{id}")]
        [Authorize(Policy = "AdminPanelAccess")]
        public async Task<IActionResult> Reject(int id)
        {
            var comment = await _commentService.GetAsync(id);
            if (comment == null) return NotFound();

            await _commentService.RejectCommentAsync(id);
            return Ok(new { message = "Comment rejected." });
        }
    }
}
