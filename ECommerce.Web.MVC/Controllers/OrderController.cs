using ECommerce.Application.Interfaces;
using ECommerce.Application.ViewModels;
using ECommerce.Data.Entities;
using Microsoft.AspNetCore.Mvc;
using System.Text.Json;

namespace ECommerce.Web.Mvc.Controllers
{
    public class OrderController : Controller
    {
        private const string SessionCartKey = "Cart";
        private const string SessionUserInfoKey = "UserInfo";

        private readonly IOrderService _orderService;
        private readonly IUserService _userService;
        private readonly ICartService _cartService;

        public OrderController(IOrderService orderService, IUserService userService, ICartService cartService)
        {
            _orderService = orderService;
            _userService = userService;
            _cartService = cartService;
        }

        private int? GetUserIdFromSession() => HttpContext.Session.GetInt32("UserId");

        private CartViewModel GetCartFromSession()
        {
            var json = HttpContext.Session.GetString(SessionCartKey);
            if (string.IsNullOrEmpty(json)) return new CartViewModel();
            return JsonSerializer.Deserialize<CartViewModel>(json) ?? new CartViewModel();
        }

        // ---------------- CREATE ORDER ----------------
        [HttpGet]
        public IActionResult Create()
        {
            var userId = GetUserIdFromSession();
            UserInformationViewModel model;

            if (userId.HasValue)
            {
                // Kullanıcı login ise DB’den bilgileri al
                var user = _userService.Get(userId.Value); // DB’den çekiyoruz
                model = new UserInformationViewModel
                {
                    FullName = $"{user.FirstName} {user.LastName}", // Burada FullName oluşturuluyor
                    Address = user.Address,
                    Phone = user.Phone,
                    Email = user.Email
                };
            }
            else
            {
                // Misafir: session’dan al
                var json = HttpContext.Session.GetString(SessionUserInfoKey);
                model = string.IsNullOrEmpty(json)
                    ? new UserInformationViewModel()
                    : JsonSerializer.Deserialize<UserInformationViewModel>(json) ?? new UserInformationViewModel();
            }

            return View(model);
        }

        [HttpPost]
        public IActionResult Create(UserInformationViewModel model)
        {
            if (!ModelState.IsValid) return View(model);

            var userId = GetUserIdFromSession();
            if (!userId.HasValue)
            {
                // Misafir: session’a kaydet
                HttpContext.Session.SetString(SessionUserInfoKey, JsonSerializer.Serialize(model));
            }
            else
            {
                // İstersen login kullanıcı için DB’de güncelleme yapılabilir
            }

            return RedirectToAction("Payment");
        }

        // ---------------- PAYMENT ----------------
        [HttpGet]
        public IActionResult Payment()
        {
            var userId = HttpContext.Session.GetInt32("UserId");
            var cartJson = HttpContext.Session.GetString("Cart");
            var cart = string.IsNullOrEmpty(cartJson) ? null : JsonSerializer.Deserialize<CartViewModel>(cartJson);

            if (!userId.HasValue && (cart == null || !cart.Items.Any()))
            {
                TempData["Message"] = "You must have items in your cart to proceed to payment.";
                return RedirectToAction("Index", "Cart");
            }

            return View(new PaymentInformationViewModel()); // default PaymentMethod = "Credit Card"
        }

        [HttpPost]
        public IActionResult Payment(PaymentInformationViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            var userId = GetUserIdFromSession();
            CartViewModel cart;

            if (userId.HasValue)
            {
                // Login: DB’den cart al
                var cartItems = _cartService.GetCartItems(userId.Value);
                cart = new CartViewModel
                {
                    Items = cartItems.Select(c => new CartItemViewModel
                    {
                        ProductId = c.ProductId,
                        Name = c.Product.Name,
                        Price = c.Product.Price,
                        Quantity = c.Quantity
                    }).ToList()
                };
            }
            else
            {
                // Misafir: session’dan cart al
                cart = GetCartFromSession();
            }

            if (!cart.Items.Any())
            {
                TempData["Message"] = "Your cart is empty.";
                return RedirectToAction("Index", "Cart");
            }

            // Kullanıcı bilgilerini al
            UserInformationViewModel userInfo;
            if (userId.HasValue)
            {
                var user = _userService.Get(userId.Value); // DB’den çekiyoruz
                userInfo = new UserInformationViewModel
                {
                    FullName = $"{user.FirstName} {user.LastName}", // Burada FullName oluşturuluyor
                    Address = user.Address,
                    Phone = user.Phone,
                    Email = user.Email
                };
            }
            else
            {
                var json = HttpContext.Session.GetString(SessionUserInfoKey);
                userInfo = string.IsNullOrEmpty(json)
                    ? new UserInformationViewModel()
                    : JsonSerializer.Deserialize<UserInformationViewModel>(json) ?? new UserInformationViewModel();
            }

            // DB’ye kaydet
            var cartItemsEntities = cart.Items.Select(i => new CartItemEntity
            {
                ProductId = i.ProductId,
                Quantity = i.Quantity,
                CreatedAt = DateTime.Now
            }).ToList();

            var order = _orderService.CreateOrder(
                userId ?? 0, // misafir için 0 veya özel guestId mantığı
                userInfo.Address,
                model.PaymentMethod,
                cartItemsEntities
            );

            // Cart temizle
            if (userId.HasValue)
                _cartService.ClearCart(userId.Value);
            else
            {
                cart.Items.Clear();
                HttpContext.Session.SetString(SessionCartKey, JsonSerializer.Serialize(cart));
            }

            return RedirectToAction("Details", new { id = order.Id });
        }

        // ---------------- ORDER DETAILS ----------------
        [HttpGet]
        [Route("order-details/{id}")]
        public IActionResult Details(int id)
        {
            var userId = GetUserIdFromSession();
            if (!userId.HasValue) return RedirectToAction("Login", "Auth");

            var order = _orderService.GetOrder(id);
            if (order == null || order.UserId != userId.Value)
                return RedirectToAction("Index", "Home");

            return View(order);
        }

        // ---------------- MY ORDERS ----------------
        [HttpGet]
        [Route("profile/my-orders")]
        public IActionResult MyOrders()
        {
            var userId = GetUserIdFromSession();
            if (!userId.HasValue) return RedirectToAction("Login", "Auth");

            var orders = _orderService.GetOrdersByUser(userId.Value);
            ViewBag.Empty = !orders.Any();
            ViewBag.PageTitle = "My Orders";

            return View(orders);
        }
    }
}