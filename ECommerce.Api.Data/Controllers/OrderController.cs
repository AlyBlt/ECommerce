using ECommerce.Application.DTOs.Order;
using ECommerce.Application.Interfaces.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ECommerce.Api.Data.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize] // Sipariş işlemleri için giriş yapmış olmak şart
    public class OrderController : ControllerBase
    {
        private readonly IOrderService _orderService;

        public OrderController(IOrderService orderService)
        {
            _orderService = orderService;
        }

        // POST: api/order/create
        [HttpPost("create")]
        public async Task<IActionResult> Create([FromBody] OrderRequestDTO request)
        {
            if (request == null || request.CartItems == null || !request.CartItems.Any())
                return BadRequest(new { message = "Invalid order request or empty cart" });

            try
            {
                var order = await _orderService.CreateOrderAsync(
                    request.UserId,
                    request.Address,
                    request.PaymentMethod,
                    request.CartItems,
                    request.DeliveryFullName,
                    request.DeliveryPhone
                );

                return Ok(order);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = "An error occurred while creating the order", detail = ex.Message });
            }
        }

        // GET: api/order/user/5
        [HttpGet("user/{userId}")]
        public async Task<IActionResult> GetByUser(int userId)
        {
            var orders = await _orderService.GetOrdersByUserAsync(userId);
            return Ok(orders);
        }

        // GET: api/order/5
        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            var order = await _orderService.GetOrderAsync(id);
            if (order == null) return NotFound();

            return Ok(order);
        }
    }
    
}
