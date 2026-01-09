using ECommerce.Application.DTOs.Cart;
using ECommerce.Application.Interfaces.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ECommerce.Api.Data.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize] // Sepet veritabanı işlemleri için yetki gerekir
    public class CartController : ControllerBase
    {
        private readonly ICartService _cartService;

        public CartController(ICartService cartService)
        {
            _cartService = cartService;
        }

        // GET: api/cart/5
        [HttpGet("{userId}")]
        public async Task<IActionResult> GetCart(int userId)
        {
            // API kendi servisini çağırırken token parametresine null verebilir.
            var items = await _cartService.GetCartItemsAsync(userId, null);
            return Ok(items);
        }

        // POST: api/cart/add/5
        [HttpPost("add/{userId}")]
        public async Task<IActionResult> AddToCart(int userId, [FromBody] CartAddDTO dto)
        {
            try
            {
                await _cartService.AddToCartAsync(userId, dto);
                return Ok(new { message = "Product added to cart successfully." });
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = "Could not add product to cart.", detail = ex.Message });
            }
        }

        // PUT: api/cart/update-item
        [HttpPut("update-item")]
        public async Task<IActionResult> UpdateItem([FromBody] CartUpdateItemRequestDTO request)
        {
            if (request == null) return BadRequest(new { message = "Invalid cart update request." });

            await _cartService.UpdateCartItemAsync(request.UserId, request.ProductId, request.Quantity);
            return Ok(new { message = "Cart updated successfully." });
        }

        // DELETE: api/cart/remove/5/12
        [HttpDelete("remove/{userId}/{productId}")]
        public async Task<IActionResult> RemoveItem(int userId, int productId)
        {
            await _cartService.RemoveCartItemAsync(userId, productId);
            return Ok(new { message = "Item removed from cart." });
        }

        // DELETE: api/cart/clear/5
        [HttpDelete("clear/{userId}")]
        public async Task<IActionResult> ClearCart(int userId)
        {
            await _cartService.ClearCartAsync(userId);
            return Ok(new { message = "Cart cleared successfully." });
        }

        [HttpPost("sync/{userId}")]
        public async Task<IActionResult> SyncCart(int userId, [FromBody] List<CartAddDTO> items)
        {
            // API tarafındayız. Veritabanına doğrudan erişen CartService'e 
            // token göndermemize gerek yok (servis içinde kullanılmıyor), 
            // ama imza uyumu için 'null' gönderiyoruz.
            await _cartService.SyncCartAsync(userId, items, null);

            return Ok(new { message = "Cart synced successfully" });
        }
    }

   
}
