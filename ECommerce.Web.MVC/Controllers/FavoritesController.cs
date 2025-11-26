using ECommerce.Application.Interfaces;
using ECommerce.Application.Services;
using ECommerce.Application.ViewModels;
using ECommerce.Web.Mvc.Helpers;
using Microsoft.AspNetCore.Mvc;
using System.Linq;

public class FavoritesController : Controller
{
    private readonly IFavoriteService _favoriteService;
    private readonly ICartService _cartService;

    private const string SessionUserIdKey = "UserId";

    public FavoritesController(IFavoriteService favoriteService, ICartService cartService)
    {
        _favoriteService = favoriteService;
        _cartService = cartService;
    }

    private int? GetCurrentUserId() => HttpContext.Session.GetInt32(SessionUserIdKey);

    public IActionResult Index()
    {
        var userId = GetCurrentUserId();
        List<FavoriteItemViewModel> favorites;

        if (userId.HasValue)
        {
            // DB’den al
            favorites = _favoriteService.GetByUser(userId.Value)
                        .Select(f => new FavoriteItemViewModel
                        {
                            ProductId = f.ProductId,
                            Name = f.Product?.Name ?? "",
                            Price = f.Product?.Price ?? 0,
                            ImageUrl = "" // Product.ImageUrl varsa ekle
                        }).ToList();
        }
        else
        {
            // Session’dan al
            favorites = SessionHelper.GetFavorites(HttpContext) ?? new List<FavoriteItemViewModel>();
        }

        HttpContext.Session.SetInt32("FavoritesCount", favorites.Count);
        return View(favorites);
    }

    [HttpPost]
    public IActionResult Add(int productId)
    {
        var userId = GetCurrentUserId();

        if (userId.HasValue)
        {
            _favoriteService.Add(userId.Value, productId);
        }
        else
        {
            // Session tabanlı ekleme
            var favorites = SessionHelper.GetFavorites(HttpContext) ?? new List<FavoriteItemViewModel>();
            if (!favorites.Any(f => f.ProductId == productId))
            {
                favorites.Add(new FavoriteItemViewModel
                {
                    ProductId = productId,
                    Name = "Demo Product", // DB yoksa placeholder
                    Price = 0,
                    ImageUrl = ""
                });
            }
            SessionHelper.SaveFavorites(HttpContext, favorites);
            HttpContext.Session.SetInt32("FavoritesCount", favorites.Count);
        }

        return Ok();
    }

    [HttpPost]
    public IActionResult Remove(int productId)
    {
        var userId = GetCurrentUserId();

        if (userId.HasValue)
        {
            _favoriteService.Remove(userId.Value, productId);
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


    [HttpPost]
    public IActionResult AddFavoritesToCart()
    {
        var userId = HttpContext.Session.GetInt32("UserId");
        if (!userId.HasValue)
            return RedirectToAction("Login", "Auth");

        // Kullanıcının favorilerini al
        var favorites = _favoriteService.GetByUser(userId.Value);

        foreach (var fav in favorites)
        {
            // Sepete ekle
            _cartService.AddToCart(userId.Value, fav.ProductId, 1); // quantity = 1
        }

        TempData["Message"] = "All favorite items added to cart.";
        return RedirectToAction("Index", "Cart");
    }
}