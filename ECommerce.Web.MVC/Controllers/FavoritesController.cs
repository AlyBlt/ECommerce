using ECommerce.Application.Interfaces.Services;
using ECommerce.Application.Services;
using ECommerce.Web.Mvc.Helpers;
using ECommerce.Web.Mvc.Models.Cart;
using ECommerce.Web.Mvc.Models.Favorite;
using ECommerce.Web.MVC.Filters;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Linq;
using System.Security.Claims;

namespace ECommerce.Web.Mvc.Controllers
{
    // Favorites işlemleri anonymous olarak yapılabilir.
    // Checkout ve Order aşamasında Buyer/Seller authorization uygulanır.
    [AllowAnonymous]
    public class FavoritesController : Controller
    {
        private readonly IFavoriteService _favoriteService;
        private readonly ICartService _cartService;
        private readonly IProductService _productService;

        public FavoritesController(IFavoriteService favoriteService, ICartService cartService, IProductService productService)
        {
            _favoriteService = favoriteService;
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

        //-------CANMODIFY HELPER-----------------
        private bool CanModifyFavorites()
        {
            // Admin favoriyi göremez, müdahale edemez
            if (User.IsInRole("Admin"))
                return false;

            // Diğer kullanıcılar (Buyer/Seller ya da anonim kullanıcılar) müdahale edebilir
            return true;
        }

        // --- INDEX ---
        public async Task<IActionResult> Index()
        {
            var userId = GetCurrentUserId();
            List<FavoriteItemViewModel> favorites;

            if (userId.HasValue)
            {
                var dbFavs = await _favoriteService.GetByUserAsync(userId.Value);
                favorites = dbFavs.Select(f => new FavoriteItemViewModel
                {
                    ProductId = f.ProductId,
                    Name = f.ProductName,
                    Price = f.ProductPrice,
                    ImageUrl = f.ProductImageUrl
                }).ToList();
            }
            else
            {
                favorites = SessionHelper.GetFavorites(HttpContext);
            }

            HttpContext.Session.SetInt32("FavoritesCount", favorites.Count);
            return View(favorites);
        }


        // --- ADD ---
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Add(int productId)
        {
            if (!CanModifyFavorites())
            {
                TempData["Error"] = "You cannot modify favorites.";
                return RedirectToAction("Index");
            }

            var userId = GetCurrentUserId();
            int favoritesCount = 0;

            if (userId.HasValue)
            {
                // Adminse favorileri sıfırla
                if (User.IsInRole("Admin"))
                {
                    SessionHelper.ClearFavorites(HttpContext);  // Adminse session favorites temizle
                }
                else
                {
                    await _favoriteService.AddAsync(userId.Value, productId);
                    var currentFavs = await _favoriteService.GetByUserAsync(userId.Value);
                    favoritesCount = currentFavs.Count();
                }
            }
            else
            {
                var favorites = SessionHelper.GetFavorites(HttpContext) ?? new List<FavoriteItemViewModel>();
                if (!favorites.Any(f => f.ProductId == productId))
                {
                    var product = await _productService.GetAsync(productId);
                    favorites.Add(new FavoriteItemViewModel
                    {
                        ProductId = productId,
                        Name = product?.Name ?? "Unknown Product",
                        Price = product?.Price ?? 0,
                        ImageUrl = product.MainImageUrl ?? "/img/product/default.jpg"
                    });
                }
                SessionHelper.SaveFavorites(HttpContext, favorites);
                favoritesCount = favorites.Count;
            }

            HttpContext.Session.SetInt32("FavoritesCount", favoritesCount);
            return Json(new { success = true, favoritesCount, message = "Added to favorites." });
        }

        // --- REMOVE ---
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Remove(int productId)
        {
            if (!CanModifyFavorites())
            {
                TempData["Error"] = "You cannot modify favorites.";
                return RedirectToAction("Index");
            }

            var userId = GetCurrentUserId();

            if (userId.HasValue)
            {
                // Adminse favorileri sıfırla
                if (User.IsInRole("Admin"))
                {
                    SessionHelper.ClearFavorites(HttpContext);  // Adminse session favorites temizle
                }
                else
                {
                    await _favoriteService.RemoveAsync(userId.Value, productId);
                    var currentFavs = await _favoriteService.GetByUserAsync(userId.Value);
                    HttpContext.Session.SetInt32("FavoritesCount", currentFavs.Count());
                }
            }
            else
            {
                var favorites = SessionHelper.GetFavorites(HttpContext) ?? new List<FavoriteItemViewModel>();
                var item = favorites.FirstOrDefault(f => f.ProductId == productId);
                if (item != null)
                {
                    favorites.Remove(item);
                    SessionHelper.SaveFavorites(HttpContext, favorites);
                }
                HttpContext.Session.SetInt32("FavoritesCount", favorites.Count);
            }

            return RedirectToAction("Index");
        }


        // --- ADD FAVORITES TO CART ---
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AddFavoritesToCart()
        {
            if (!CanModifyFavorites())
            {
                TempData["Error"] = "You cannot modify favorites.";
                return RedirectToAction("Index");
            }

            var userId = GetCurrentUserId();
            int totalCount = 0;
            decimal totalPrice = 0;

            if (userId.HasValue)
            {
                // Adminse favorileri sıfırla
                if (User.IsInRole("Admin"))
                {
                    SessionHelper.ClearFavorites(HttpContext);  // Adminse session favorites temizle
                }
                else
                {
                    var favorites = await _favoriteService.GetByUserAsync(userId.Value);
                    foreach (var fav in favorites)
                    {
                        await _cartService.AddToCartAsync(userId.Value, new ECommerce.Application.DTOs.Cart.CartAddDTO
                        {
                            ProductId = fav.ProductId,
                            Quantity = 1
                        });
                    }
                    await _favoriteService.ClearAsync(userId.Value);

                    var cartItems = await _cartService.GetCartItemsAsync(userId.Value);
                    totalCount = cartItems.Sum(x => x.Quantity);
                    totalPrice = cartItems.Sum(x => x.Quantity * x.Price);
                }
            }
            else
            {
                var favorites = SessionHelper.GetFavorites(HttpContext) ?? new List<FavoriteItemViewModel>();
                foreach (var fav in favorites)
                {
                    SessionHelper.AddToCart(HttpContext, fav.ProductId, fav.Name, fav.Price, 1, fav.ImageUrl);
                }
                SessionHelper.ClearFavorites(HttpContext);

                var cart = SessionHelper.GetCart(HttpContext);
                totalCount = cart.Items.Sum(i => i.Quantity);
                totalPrice = cart.Items.Sum(i => i.Quantity * i.Price);
            }

            HttpContext.Session.SetInt32("CartCount", totalCount);
            HttpContext.Session.SetString("CartTotal", totalPrice.ToString("0.00", System.Globalization.CultureInfo.InvariantCulture));
            HttpContext.Session.SetInt32("FavoritesCount", 0);

            return Json(new
            {
                success = true,
                message = "All favorite items moved to cart.",
                cartCount = totalCount,
                cartTotal = totalPrice
            });
        }

        // --- CLEAR ALL ---
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ClearFavorites()
        {
            if (!CanModifyFavorites())
            {
                TempData["Error"] = "You cannot modify favorites.";
                return RedirectToAction("Index");
            }

            var userId = GetCurrentUserId();

            if (userId.HasValue)
            {
                await _favoriteService.ClearAsync(userId.Value);
            }
            else
            {
                SessionHelper.ClearFavorites(HttpContext);
            }

            HttpContext.Session.SetInt32("FavoritesCount", 0);
            TempData["Success"] = "Your favorites list is now empty.";
            return RedirectToAction("Index");
        }
    }
}