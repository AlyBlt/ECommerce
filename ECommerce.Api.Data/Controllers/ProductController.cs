using ECommerce.Application.DTOs.Product;
using ECommerce.Application.DTOs.ProductComment;
using ECommerce.Application.Interfaces.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ECommerce.Api.Data.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProductController : ControllerBase
    {
        private readonly IProductService _productService;

        public ProductController(IProductService productService)
        {
            _productService = productService;
        }

        // --- TEMEL OKUMA (GET) ---

        [HttpGet]
        public async Task<IActionResult> GetAll() => Ok(await _productService.GetAllAsync());

        [HttpGet("{id}")]
        public async Task<IActionResult> Get(int id)
        {
            var product = await _productService.GetAsync(id);
            return product == null ? NotFound() : Ok(product);
        }

        // --- ÖZEL LİSTELEMELER (Web Home Page) ---

        [HttpGet("active")]
        public async Task<IActionResult> GetActive() => Ok(await _productService.GetActiveProductsAsync());

        [HttpGet("featured")]
        public async Task<IActionResult> GetFeatured() => Ok(await _productService.GetFeaturedProductsAsync());

        [HttpGet("discounted")]
        public async Task<IActionResult> GetDiscounted() => Ok(await _productService.GetDiscountedProductsAsync());

        [HttpGet("new-arrivals")]
        public async Task<IActionResult> GetNewArrivals() => Ok(await _productService.GetNewArrivalProductsAsync());

        [HttpGet("related/{categoryId}/{currentProductId}")]
        public async Task<IActionResult> GetRelated(int categoryId, int currentProductId)
            => Ok(await _productService.GetRelatedProductsAsync(categoryId, currentProductId));

        // --- ARAMA ---

        [HttpGet("search")]
        public async Task<IActionResult> Search([FromQuery] string query, [FromQuery] string? category, [FromQuery] byte? rating)
            => Ok(await _productService.SearchProductsAsync(query, category, rating));

        // --- YAZMA İŞLEMLERİ (Authorize Gerekir) ---

        [Authorize(Roles = "Seller, Admin, SystemAdmin")]
        [HttpPost]
        public async Task<IActionResult> Create([FromBody] ProductDTO dto)
        {
            await _productService.AddAsync(dto);
            return Ok();
        }

        [Authorize(Roles = "Seller,Admin, SystemAdmin")]
        [HttpPut("{id}")] // URL'den ID alacak şekilde güncelledik
        public async Task<IActionResult> Update(int id, [FromBody] ProductDTO dto)
        {
            dto.Id = id; // Güvenlik için URL'deki ID'yi DTO'ya basıyoruz
            await _productService.UpdateAsync(dto);
            return NoContent();
        }

        [Authorize(Roles = "Seller,Admin, SystemAdmin")]
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            await _productService.DeleteAsync(id);
            return NoContent();
        }

        [Authorize(Roles = "Seller,Admin, SystemAdmin")]
        [HttpPost("toggle/{id}")]
        public async Task<IActionResult> ToggleStatus(int id)
        {
            await _productService.ToggleStatusAsync(id);
            return Ok();
        }

        // --- YORUM VE SATIN ALMA KONTROLÜ ---

        [Authorize] // Giriş yapan herkes (Buyer/Seller) yorum atabilir
        [HttpPost("{productId}/comment")]
        public async Task<IActionResult> AddComment(int productId, [FromBody] ProductCommentDTO commentDto)
        {
            try
            {
                // IActionResult olan metodu değil, Service içindeki bool dönen metodu çağırıyoruz:
                bool hasPurchased = await _productService.HasUserPurchasedProductAsync(commentDto.UserId, productId);

                if (!hasPurchased)
                {
                    return BadRequest(new { message = "You must purchase the product before commenting." });
                }

                await _productService.AddCommentAsync(productId, commentDto);
                return Ok(new { message = "Comment submitted successfully" });
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpGet("check-purchase/{userId}/{productId}")]
        public async Task<IActionResult> CheckPurchase(int userId, int productId)
            => Ok(await _productService.HasUserPurchasedProductAsync(userId, productId));
    }
}
