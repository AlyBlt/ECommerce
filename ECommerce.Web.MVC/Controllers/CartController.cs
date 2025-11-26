using ECommerce.Application.Interfaces;
using ECommerce.Application.ViewModels;
using Microsoft.AspNetCore.Mvc;
using System.Linq;

namespace ECommerce.Web.Mvc.Controllers
{
    public class CartController : Controller
    {
        private readonly ICartService _cartService;
        private readonly IUserService _userService;
        private const string SessionUserId = "UserId";

        public CartController(ICartService cartService, IUserService userService)
        {
            _cartService = cartService;
            _userService = userService;
        }

        private int? GetCurrentUserId() => HttpContext.Session.GetInt32(SessionUserId);

        // ---------------- CART INDEX ----------------
        public IActionResult Index()
        {
            var userId = GetCurrentUserId();
            if (!userId.HasValue) return RedirectToAction("Login", "Auth");

            var cartItems = _cartService.GetCartItems(userId.Value)
                .Select(c => new CartItemViewModel
                {
                    ProductId = c.ProductId,
                    Name = c.Product.Name,
                    Price = c.Product.Price,
                    Quantity = c.Quantity
                })
                .ToList();

            HttpContext.Session.SetInt32("CartCount", cartItems.Sum(c => c.Quantity));

            var model = new CartViewModel
            {
                Items = cartItems
            };

            return View(model); // artık CartViewModel gönderiyoruz
        }


        // ---------------- ADD PRODUCT ----------------
        [HttpPost]
        public IActionResult AddProduct(int productId, int quantity = 1)
        {
            var userId = GetCurrentUserId();
            if (!userId.HasValue) return RedirectToAction("Login", "Auth");

            _cartService.AddToCart(userId.Value, productId, quantity);

            var cartItems = _cartService.GetCartItems(userId.Value)
                .Select(c => new CartItemViewModel
                {
                    ProductId = c.ProductId,
                    Name = c.Product.Name,
                    Price = c.Product.Price,
                    Quantity = c.Quantity
                })
                .ToList();

            HttpContext.Session.SetInt32("CartCount", cartItems.Sum(c => c.Quantity));

            TempData["Message"] = "Product added to the cart.";
            return RedirectToAction("Index");
        }

        // ---------------- EDIT CART ----------------
        [HttpPost]
        public IActionResult Edit(int productId, int quantity)
        {
            var userId = GetCurrentUserId();
            if (!userId.HasValue) return RedirectToAction("Login", "Auth");

            // ------------------ GÜVENLİLİK KONTROLÜ ------------------
            if (quantity < 0) quantity = 1;  // Negatif veya hatalı değerleri düzelt
            if (quantity == 0)
            {
                _cartService.RemoveCartItem(userId.Value, productId);
                TempData["Message"] = "Product removed from cart.";
            }
            else
            {
                _cartService.UpdateCartItem(userId.Value, productId, quantity);
            }
            // ---------------------------------------------------------

            var cartItems = _cartService.GetCartItems(userId.Value)
                .Select(c => new CartItemViewModel
                {
                    ProductId = c.ProductId,
                    Name = c.Product.Name,
                    Price = c.Product.Price,
                    Quantity = c.Quantity
                })
                .ToList();

            HttpContext.Session.SetInt32("CartCount", cartItems.Sum(c => c.Quantity));

            return RedirectToAction("Index");
        }

        // ---------------- CLEAR CART ----------------
        [HttpPost]
        public IActionResult Clear()
        {
            var userId = GetCurrentUserId();
            if (!userId.HasValue) return RedirectToAction("Login", "Auth");

            _cartService.ClearCart(userId.Value);
            HttpContext.Session.SetInt32("CartCount", 0);

            TempData["Message"] = "Your cart has been cleared.";
            return RedirectToAction("Index");
        }
    }
}