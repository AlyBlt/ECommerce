using ECommerce.Application.DTOs.Cart;
using ECommerce.Application.Interfaces.Services;
using ECommerce.Web.Mvc.Helpers;
using ECommerce.Web.Mvc.Models.Cart;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace ECommerce.Web.Mvc.Controllers
{
    // Cart işlemleri anonymous olarak yapılabilir.
    // Checkout ve Order aşamasında Buyer/Seller authorization uygulanır.
    [AllowAnonymous]
    public class CartController : Controller
    {
        private readonly ICartService _cartApiService;
        private readonly IProductService _productApiService;

        public CartController(ICartService cartApiService, IProductService productApiService)
        {
            _cartApiService = cartApiService;
            _productApiService = productApiService;
        }

        // ---------------- CURRENT USER ----------------
        private int? GetCurrentUserId()
        {
            if (User.Identity?.IsAuthenticated != true) return null;
            var claim = User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier);
            return claim != null ? int.Parse(claim.Value) : null;
        }

        private bool CanModifyCart()
        {
            // Admin ve SystemAdmin rollerinden herhangi birine sahipse false döner
            return !(User.IsInRole("Admin") || User.IsInRole("SystemAdmin"));
        }

        // ---------------- CART INDEX ----------------
        public async Task<IActionResult> Index()
        {
            var userId = GetCurrentUserId();
            List<CartItemViewModel> cartItems;

            if (userId.HasValue)
            {
                // Get from API (Database)
                var dtos = await _cartApiService.GetCartItemsAsync(userId.Value);
                cartItems = dtos.Select(d => new CartItemViewModel
                {
                    ProductId = d.ProductId,
                    Name = d.Name,
                    Price = d.Price,
                    Quantity = d.Quantity,
                    ImageUrl = d.ImageUrl
                }).ToList();
            }
            else
            {
                var cart = SessionHelper.GetCart(HttpContext);
                cartItems = cart.Items;
            }

            HttpContext.Session.SetInt32("CartCount", cartItems.Sum(c => c.Quantity));
            HttpContext.Session.SetString("CartTotal", cartItems.Sum(c => c.Quantity * c.Price).ToString("0.00"));

            return View(new CartViewModel
            {
                Items = cartItems
            });
        }

        // ---------------- ADD PRODUCT ----------------
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AddProduct( int productId, int quantity = 1)
        {
            if (!CanModifyCart())
            {
                return Json(new { success = false, message = "Admins cannot perform shopping actions." });
            }

            if (quantity <= 0)
                quantity = 1;

           
            // Ürün bilgisi HER ZAMAN server’dan alınır
            var product = await _productApiService.GetAsync(productId);
            if (product == null || !product.Enabled)
            {
                return Json(new
                {
                    success = false,
                    message = "This product is not available."
                });
            }

            var userId = GetCurrentUserId();
            int totalCount = 0;
            decimal totalPrice = 0;

            if (userId.HasValue)
            {
                // Logged in: Add to Database via API
                await _cartApiService.AddToCartAsync(userId.Value, new CartAddDTO
                {
                    ProductId = productId,
                    Quantity = (byte)quantity
                });
                var dtos = await _cartApiService.GetCartItemsAsync(userId.Value);
                totalCount = dtos.Sum(d => d.Quantity);
                totalPrice = dtos.Sum(d => d.TotalPrice);
               
            }
            else
            {
                // anonim kullanıcılar için sepet işlemleri    
                SessionHelper.AddToCart(HttpContext, new CartItemViewModel
                {
                    ProductId = product.Id,
                    Name = product.Name,
                    Price = product.Price,
                    Quantity = (byte)quantity,
                    ImageUrl = product.MainImageUrl
                });


                var cart = SessionHelper.GetCart(HttpContext);
                totalCount = cart.Items.Sum(c => c.Quantity);
                totalPrice = cart.TotalPrice;
            }
          

            HttpContext.Session.SetInt32("CartCount", totalCount);
            HttpContext.Session.SetString("CartTotal", totalPrice.ToString("0.00", System.Globalization.CultureInfo.InvariantCulture));

            // JSON’da decimal olarak gönderiyoruz
            return Json(new
            {
                success = true,
                message = "Product added to cart.",
                cartCount = totalCount,
                cartTotal = totalPrice // decimal olarak bırak
            });
        }

        // ---------------- EDIT CART ITEM ----------------
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int productId, int quantity)
        {
            if (!CanModifyCart())
            {
                TempData["ErrorAdmin"] = "You cannot modify the cart.";
                return RedirectToAction("Index");
            }

            if (quantity < 0) quantity = 0;
            if (quantity > 255) quantity = 255;

            var userId = GetCurrentUserId();
            int totalCount = 0;
            decimal totalPrice = 0;

            if (userId.HasValue)
            {
                if (quantity == 0)
                {
                    await _cartApiService.RemoveCartItemAsync(userId.Value, productId);
                }
                else
                {
                    await _cartApiService.UpdateCartItemAsync(userId.Value, productId, quantity);
                }

                var dtos = await _cartApiService.GetCartItemsAsync(userId.Value);
                totalCount = dtos.Sum(c => c.Quantity);
                totalPrice = dtos.Sum(c => c.TotalPrice);
            }
            else
            {
                var cart = SessionHelper.GetCart(HttpContext);
                var item = cart.Items.FirstOrDefault(c => c.ProductId == productId);

                if (item != null)
                {
                    if (quantity == 0)
                        cart.Items.Remove(item);
                    else
                        item.Quantity = (byte)quantity;

                    SessionHelper.SaveCart(HttpContext, cart);
                }

                totalCount = cart.Items.Sum(c => c.Quantity);
                totalPrice = cart.Items.Sum(c => c.Quantity * c.Price);
            }

            HttpContext.Session.SetInt32("CartCount", totalCount);
            HttpContext.Session.SetString("CartTotal", totalPrice.ToString("0.00", System.Globalization.CultureInfo.InvariantCulture));

            return RedirectToAction("Index");  
        }

        // ---------------- CLEAR CART ----------------
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Clear()
        {
            if (!CanModifyCart())
            {
                TempData["ErrorAdmin"] = "You cannot modify the cart.";
                return RedirectToAction("Index");
            }

            var userId = GetCurrentUserId();

            if (userId.HasValue)
            {
                await _cartApiService.ClearCartAsync(userId.Value);
            }
            else
            {
                SessionHelper.ClearCart(HttpContext); // Diğer kullanıcılar için sepeti temizle
            }

            HttpContext.Session.SetInt32("CartCount", 0);
            HttpContext.Session.SetString("CartTotal", 0.00.ToString("0.00", System.Globalization.CultureInfo.InvariantCulture));

            TempData["Success"] = "Cart cleared successfully.";

            return RedirectToAction("Index"); 
        }
    }
}