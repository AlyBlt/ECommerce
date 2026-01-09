using ECommerce.Application.Interfaces.Services;
using ECommerce.Web.Mvc.Helpers;
using ECommerce.Web.Mvc.Models.Favorite;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace ECommerce.Web.Mvc.Controllers
{
    // Favorites işlemleri anonymous olarak yapılabilir.
    // Checkout ve Order aşamasında Buyer/Seller authorization uygulanır.
    [AllowAnonymous]
    public class FavoritesController : Controller
    {
        private readonly IFavoriteService _favoriteApiService;
        private readonly ICartService _cartApiService;
        private readonly IProductService _productApiService;

        public FavoritesController(IFavoriteService favoriteApiService, ICartService cartApiService, IProductService productApiService)
        {
            _favoriteApiService = favoriteApiService;
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

        private bool CanModifyFavorites()
        {
            // Admin ve SystemAdmin rollerinden herhangi birine sahipse false döner
            return !(User.IsInRole("Admin") || User.IsInRole("SystemAdmin"));
        }

        // --- INDEX ---
        public async Task<IActionResult> Index()
        {
            var userId = GetCurrentUserId();
            List<FavoriteItemViewModel> favorites;

            if (userId.HasValue)
            {
                var dbFavs = await _favoriteApiService.GetByUserAsync(userId.Value);
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

                await _favoriteApiService.AddAsync(userId.Value, productId);
                var currentFavs = await _favoriteApiService.GetByUserAsync(userId.Value);
                favoritesCount = currentFavs.Count();
            
            }
            else
            {
                var favorites = SessionHelper.GetFavorites(HttpContext) ?? new List<FavoriteItemViewModel>();
                if (!favorites.Any(f => f.ProductId == productId))
                {
                    var product = await _productApiService.GetAsync(productId);
                    favorites.Add(new FavoriteItemViewModel
                    {
                        ProductId = productId,
                        Name = product?.Name ?? "Unknown Product",
                        Price = product?.Price ?? 0,
                        ImageUrl = product?.MainImageUrl
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

                await _favoriteApiService.RemoveAsync(userId.Value, productId);
                var currentFavs = await _favoriteApiService.GetByUserAsync(userId.Value);
                HttpContext.Session.SetInt32("FavoritesCount", currentFavs.Count());
                
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
                // NOT: Şu anki yetkilendirme filtreleri nedeniyle Admin ve SystemAdmin mağazaya giriş yapamıyor.
                // Ancak gelecekte giriş izni verilirse, güvenli bir geçiş yapılmasını sağlar.-->şimdilik dursun
                // Admin veya SystemAdmin ise favorileri sıfırla
                if (User.IsInRole("Admin") || User.IsInRole("SystemAdmin"))
                {
                    SessionHelper.ClearFavorites(HttpContext);  // Session'daki favori listesini temizle
                }
                else
                {
                    var favorites = await _favoriteApiService.GetByUserAsync(userId.Value);
                    foreach (var fav in favorites)
                    {
                        await _cartApiService.AddToCartAsync(userId.Value, new ECommerce.Application.DTOs.Cart.CartAddDTO
                        {
                            ProductId = fav.ProductId,
                            Quantity = 1
                        });
                    }
                    await _favoriteApiService.ClearAsync(userId.Value);

                    var cartItems = await _cartApiService.GetCartItemsAsync(userId.Value);
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
                await _favoriteApiService.ClearAsync(userId.Value);
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