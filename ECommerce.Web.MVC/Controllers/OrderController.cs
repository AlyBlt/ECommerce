using ECommerce.Application.Interfaces.Services;
using ECommerce.Domain.Entities;
using ECommerce.Web.Mvc.Models.Cart;
using ECommerce.Web.Mvc.Models.Order;
using ECommerce.Web.Mvc.Models.User;
using ECommerce.Web.MVC.Filters;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using System.Text.Json;

namespace ECommerce.Web.Mvc.Controllers
{
    [Authorize(Roles = "Seller,Buyer")]
    [ActiveUserAuthorize]
    public class OrderController : Controller
    {
        private readonly IOrderService _orderService;
        private readonly IUserService _userService;
        private readonly ICartService _cartService;

        public OrderController(IOrderService orderService, IUserService userService, ICartService cartService)
        {
            _orderService = orderService;
            _userService = userService;
            _cartService = cartService;
        }

        // Cookie Authentication içindeki NameIdentifier (UserId) claim'ini okur
        private int GetCurrentUserId()
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);
            return int.Parse(userIdClaim!.Value);
        }

              
        // ---------------- CREATE ORDER ----------------
        [HttpGet]
        public async Task<IActionResult> Create()
        {
            var userId = GetCurrentUserId();

            // Kullanıcı bilgilerini çek
            var user = await _userService.GetAsync(userId);
            if (user == null) return RedirectToAction("Login", "Auth");

            var model = new UserInformationViewModel
            {
                FullName = $"{user.FirstName} {user.LastName}",
                Address = user.Address,
                Phone = user.Phone,
                Email = user.Email
            };

            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create(UserInformationViewModel model)
        {
            if (!ModelState.IsValid)
                return View(model);

            // User bilgilerini sonraki adım (Payment) için session'a geçici olarak kaydediyoruz
            HttpContext.Session.SetString("UserInfo", JsonSerializer.Serialize(model));

            return RedirectToAction("Payment");
        }

        // ---------------- PAYMENT (GET) ----------------
        [HttpGet]
        public async Task<IActionResult> Payment()
        {
            var userId = GetCurrentUserId();

            // sepet öğelerini al
            var cartItems = await _cartService.GetCartItemsAsync(userId);
            if (!cartItems.Any())
            {
                TempData["Message"] = "Your cart is empty.";
                return RedirectToAction("Index", "Cart");
            }

            var cartViewModel = new CartViewModel
            {
                Items = cartItems.Select(c => new CartItemViewModel
                {
                    ProductId = c.ProductId,
                    Name = c.Name,
                    Price = c.Price,
                    Quantity = (byte)c.Quantity,
                    ImageUrl = c.ImageUrl ?? "/img/product/default.jpg"
                }).ToList()
            };

            ViewBag.Cart = cartViewModel;
            ViewBag.Total = cartViewModel.Items.Sum(i => i.Price * i.Quantity);

            return View(new PaymentInformationViewModel());
        }

        // ---------------- PAYMENT (POST - ORDER COMPLETION) ----------------
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Payment(PaymentInformationViewModel model)
        {
            var userId = GetCurrentUserId();

            if (!ModelState.IsValid)
            {
                var items = await _cartService.GetCartItemsAsync(userId);

                ViewBag.Cart = new CartViewModel
                {
                    Items = items.Select(c => new CartItemViewModel
                    {
                        ProductId = c.ProductId,
                        Name = c.Name,
                        Price = c.Price,
                        Quantity = (byte)c.Quantity,
                        ImageUrl = c.ImageUrl ?? "/img/product/default.jpg"
                    }).ToList()
                };

                ViewBag.Total = items.Sum(i => i.Price * i.Quantity);
                return View(model);
            }

            // Sepeti çek
            var cartItems = await _cartService.GetCartItemsAsync(userId);
            if (!cartItems.Any())
            {
                TempData["Message"] = "Your cart is empty.";
                return RedirectToAction("Index", "Cart");
            }

            // Session'dan kullanıcı bilgilerini (adres vb.) çek
            var userInfoJson = HttpContext.Session.GetString("UserInfo");
            var userInfo = string.IsNullOrEmpty(userInfoJson)
                ? null
                : JsonSerializer.Deserialize<UserInformationViewModel>(userInfoJson);

            // Session'daki user bilgisi veya profil adresi değil
            // Payment formunda girilen DeliveryAddress alınmalı
            // Payment formunda girilen tüm bilgileri al
            string finalAddress = model.DeliveryAddress;
            string deliveryFullName = model.DeliveryFullName;
            string deliveryPhone = model.DeliveryPhone;

            // Siparişi asenkron olarak oluştur
            // ARTIK BURADA ENTITY NEWLEMIYORUZ! 
            // Doğrudan cartItems (DTO listesi) gönderiyoruz.

            var order = await _orderService.CreateOrderAsync(
                userId,
                model.DeliveryAddress,
                model.PaymentMethod,
                cartItems.ToList(), // Doğrudan DTO listesini gönderiyoruz
                model.DeliveryFullName,
                model.DeliveryPhone
            );

            // DB'den sepeti temizle ve Session sayacını sıfırla
            await _cartService.ClearCartAsync(userId);
            HttpContext.Session.SetInt32("CartCount", 0);
            HttpContext.Session.Remove("UserInfo"); // Geçici bilgiyi temizle

            return RedirectToAction("Details", new { id = order.Id });
        }

        // ---------------- ORDER DETAILS ----------------
        [HttpGet]
        [Route("order-details/{id}")]
        public async Task<IActionResult> Details(int id)
        {
            var userId = GetCurrentUserId();
            var orderDto = await _orderService.GetOrderAsync(id);

            // Güvenlik kontrolü: Sipariş var mı ve bu kullanıcıya mı ait?
            if (orderDto == null || orderDto.UserId != userId)
                return RedirectToAction("Index", "Home");

            var model = new OrderDetailsViewModel
            {
                Id = orderDto.Id,
                OrderCode = orderDto.OrderCode,
                CreatedAt = orderDto.CreatedAt,
                PaymentMethod = orderDto.PaymentMethod,
                TotalPrice = orderDto.TotalPrice,
                DeliveryAddress = orderDto.DeliveryAddress,
                DeliveryFullName = orderDto.DeliveryFullName, 
                DeliveryPhone = orderDto.DeliveryPhone,    
                UserInformation = new UserInformationViewModel
                {
                    FullName = $"{orderDto.UserFullName}",
                    Email = orderDto.UserEmail,
                    Phone = orderDto.UserPhone,
                    Address = orderDto.UserAddress 
                },
                PaymentInformation = new PaymentInformationViewModel
                {
                    DeliveryAddress = orderDto.DeliveryAddress,
                    PaymentMethod = orderDto.PaymentMethod,
                    DeliveryFullName = orderDto.DeliveryFullName,
                    DeliveryPhone = orderDto.DeliveryPhone
                },
                Items = orderDto.Items.Select(i => new OrderItemViewModel
                {
                    ProductName = i.ProductName,
                    UnitPrice = i.UnitPrice,
                    Quantity = i.Quantity,
                    ImageUrl = i.ImageUrl,
                }).ToList()
            };

            return View(model);
        }

    }
}