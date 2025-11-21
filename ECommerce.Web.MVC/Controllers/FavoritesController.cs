using ECommerceWeb.MVC.Models;
using ECommerceWeb.MVC.Helpers;
using Microsoft.AspNetCore.Mvc;
using System.Linq;
using ECommerceWeb.MVC.Models.FavoritesViewModels;
using ECommerceWeb.MVC.Models.HomeViewModels;

namespace ECommerceWeb.MVC.Controllers
{
    public class FavoritesController : Controller
    {
        // Favoriler sayfasını görüntüle
        public IActionResult Index()
        {
            var favorites = SessionHelper.GetFavorites(HttpContext) ?? new List<FavoriteItem>();
            HttpContext.Session.SetInt32("FavoritesCount", favorites.Count);
            return View(favorites);
        }

        // Ürünü favorilere ekle
        [HttpPost]
        public IActionResult Add(int productId)
        {
            var favorites = SessionHelper.GetFavorites(HttpContext) ?? new List<FavoriteItem>();

            if (!favorites.Any(f => f.ProductId == productId))
            {
                var products = HttpContext.Session.GetObjectFromJson<List<ProductListingModel>>("Products") ?? new List<ProductListingModel>();
                var product = products.FirstOrDefault(p => p.Id == productId);

                if (product != null)
                {
                    favorites.Add(new FavoriteItem
                    {
                        ProductId = product.Id,
                        Name = product.Name,
                        Price = product.Price,
                        ImageUrl = product.ImageUrl
                    });
                }
            }

            SessionHelper.SaveFavorites(HttpContext, favorites);
            HttpContext.Session.SetInt32("FavoritesCount", favorites.Count);

            return Ok(); // JS için
        }

        // Ürünü favorilerden kaldır
        [HttpPost]
        public IActionResult Remove(int productId)
        {
            var favorites = SessionHelper.GetFavorites(HttpContext) ?? new List<FavoriteItem>();
            var item = favorites.FirstOrDefault(f => f.ProductId == productId);

            if (item != null)
            {
                favorites.Remove(item);
                SessionHelper.SaveFavorites(HttpContext, favorites);
            }

            HttpContext.Session.SetInt32("FavoritesCount", favorites.Count);

            return RedirectToAction("Index");
        }
    }
}