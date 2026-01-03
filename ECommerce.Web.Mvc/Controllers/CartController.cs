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
        private readonly ICartService _cartService;
        private readonly IProductService _productService;

        public CartController(ICartService cartService, IProductService productService)
        {
            _cartService = cartService;
            _productService = productService;
        }

        // ---------------- CURRENT USER ----------------
        private int? GetCurrentUserId()
        {
            if (User.Identity?.IsAuthenticated != true)
                return null;

            var claim = User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier);
            return claim != null ? int.Parse(claim.Value) : null;
        }

        private bool CanModifyCart()
        {
            // Admin favoriyi göremez, müdahale edemez
            if (User.IsInRole("Admin"))
                return false;

            // Diğer kullanıcılar (Buyer/Seller ya da anonim kullanıcılar) müdahale edebilir
            return true;
        }

        // ---------------- CART INDEX ----------------
        public async Task<IActionResult> Index()
        {
            var userId = GetCurrentUserId();
            List<CartItemViewModel> cartItems;

            if (userId.HasValue)
            {
                var dtos = await _cartService.GetCartItemsAsync(userId.Value);
                // DTO -> ViewModel Mapping
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
                TempData["ErrorAdmin"] = "You cannot modify the cart.";
                return RedirectToAction("Index");
            }

            if (quantity <= 0)
                quantity = 1;

            var userId = GetCurrentUserId();
            int totalCount = 0;
            decimal totalPrice = 0;

            // Ürün bilgisi HER ZAMAN server’dan alınır
            var product = await _productService.GetAsync(productId);
            if (product == null || !product.Enabled)
            {
                return Json(new
                {
                    success = false,
                    message = "This product is not available."
                });
            }

           
            if (userId.HasValue)
            {
                // Adminse sepeti sıfırla
                if (User.IsInRole("Admin"))
                {
                    SessionHelper.ClearCart(HttpContext);  // Adminse session sepeti temizle
                }
                else
                {
                    await _cartService.AddToCartAsync(userId.Value, new CartAddDTO
                    {
                        ProductId = productId,
                        Quantity = (byte)quantity
                    });

                    var dtos = await _cartService.GetCartItemsAsync(userId.Value);
                    totalCount = dtos.Sum(d => d.Quantity);
                    totalPrice = dtos.Sum(d => d.TotalPrice);
                }
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
                if (User.IsInRole("Admin"))
                {
                    SessionHelper.ClearCart(HttpContext); // Admin için sepetteki ürünleri temizle
                }
                if (quantity == 0)
                {
                    await _cartService.RemoveCartItemAsync(userId.Value, productId);
                }
                else
                {
                    await _cartService.UpdateCartItemAsync(userId.Value, productId, quantity);
                }

                var dtos = await _cartService.GetCartItemsAsync(userId.Value);
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
                await _cartService.ClearCartAsync(userId.Value);
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