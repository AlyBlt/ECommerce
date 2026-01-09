using ECommerce.Application.DTOs.Favorite;
using ECommerce.Application.Interfaces.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ECommerce.Api.Data.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize] // Favori işlemleri DB seviyesinde authorize olmalı (Kullanıcı login olmalı)
    public class FavoriteController : ControllerBase
    {
        private readonly IFavoriteService _favoriteService;

        public FavoriteController(IFavoriteService favoriteService)
        {
            _favoriteService = favoriteService;
        }

        // Kullanıcının tüm favorilerini getirir
        [HttpGet("{userId}")]
        public async Task<IActionResult> GetByUser(int userId)
         => Ok(await _favoriteService.GetByUserAsync(userId, null)); // null eklendi

        // Tek bir ürünü favorilere ekler
        [HttpPost("add")]
        public async Task<IActionResult> Add([FromBody] FavoriteRequestDTO req)
        {
            await _favoriteService.AddAsync(req.UserId, req.ProductId);
            return Ok(new { message = "Product added to favorites." });
        }

        // --- LOGIN SENKRONİZASYONU ---
        // Web MVC login olduğunda session'daki favorileri buraya toplu POST eder
        [HttpPost("batch-add")]
        public async Task<IActionResult> BatchAdd([FromBody] FavoriteBatchRequestDTO model)
        {
            if (model.ProductIds == null || !model.ProductIds.Any())
                return BadRequest("Product list is empty.");

            foreach (var productId in model.ProductIds)
            {
                // Service içindeki AddAsync zaten "Exists" kontrolü yapıyorsa mükerrer kayıt oluşmaz
                await _favoriteService.BatchAddAsync(model.UserId, model.ProductIds, null); // null eklendi
            }
            return Ok(new { message = "Session favorites synced with database." });
        }

        // Favorilerden ürün siler
        [HttpDelete("remove/{userId}/{productId}")]
        public async Task<IActionResult> Remove(int userId, int productId)
        {
            await _favoriteService.RemoveAsync(userId, productId);
            return NoContent();
        }

        // Kullanıcının tüm favori listesini temizler
        [HttpDelete("clear/{userId}")]
        public async Task<IActionResult> Clear(int userId)
        {
            await _favoriteService.ClearAsync(userId);
            return NoContent();
        }

        // Bir ürünün favorilerde olup olmadığını kontrol eder
        [HttpGet("exists/{userId}/{productId}")]
        public async Task<IActionResult> Exists(int userId, int productId)
            => Ok(await _favoriteService.ExistsAsync(userId, productId));
    }

   
}
