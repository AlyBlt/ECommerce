using Microsoft.AspNetCore.Http;
using System.Collections.Generic;
using ECommerce.Web.Mvc.Models.Favorite;
using ECommerce.Web.Mvc.Models.Cart;

namespace ECommerce.Web.Mvc.Helpers
{
    // Bu metot, sepet ve favoriler için session yönetimini sağlar
    public static class SessionHelper
    {
        private const string SessionCartKey = "Cart";
        private const string SessionFavoritesKey = "Favorites";

        // Sepet session'ını almak
        public static CartViewModel GetCart(HttpContext httpContext)
        {
            return httpContext.Session.GetObjectFromJson<CartViewModel>(SessionCartKey) ?? new CartViewModel();
        }

        // Sepet session'ını kaydetmek
        public static void SaveCart(HttpContext httpContext, CartViewModel cart)
        {
            httpContext.Session.SetObjectAsJson(SessionCartKey, cart);
        }

        // Sepet session'ını temizlemek
        public static void ClearCart(HttpContext httpContext)
        {
            httpContext.Session.Remove(SessionCartKey);
        }

        // Favoriler session'ını almak
        public static List<FavoriteItemViewModel> GetFavorites(HttpContext httpContext)
        {
            return httpContext.Session.GetObjectFromJson<List<FavoriteItemViewModel>>(SessionFavoritesKey)
                   ?? new List<FavoriteItemViewModel>();
        }

        // Favoriler session'ını kaydetmek
        public static void SaveFavorites(HttpContext httpContext, List<FavoriteItemViewModel> favorites)
        {
            httpContext.Session.SetObjectAsJson(SessionFavoritesKey, favorites);
        }

        // Favoriler session'ını temizlemek
        public static void ClearFavorites(HttpContext httpContext)
        {
            httpContext.Session.Remove(SessionFavoritesKey);
        }

        // Add to cart with session (login olmadan)
         public static void AddToCart(HttpContext httpContext, int productId, string name, decimal price, int quantity = 1, string? imageUrl = null)
        {
            var cart = GetCart(httpContext);

            // Ürün zaten varsa miktarı artır
            var existingItem = cart.Items.Find(i => i.ProductId == productId);
            if (existingItem != null)
            {
                existingItem.Quantity += (byte)quantity;
            }
            else
            {
                cart.Items.Add(new CartItemViewModel
                {
                    ProductId = productId,
                    Name = name,
                    Price = price,
                    Quantity = (byte)quantity,
                    ImageUrl = imageUrl

                });
            }

            SaveCart(httpContext, cart);
        }

        public static void AddToCart(HttpContext httpContext, CartItemViewModel item)
        {
            var cart = GetCart(httpContext);

            var existingItem = cart.Items.FirstOrDefault(i => i.ProductId == item.ProductId);
            if (existingItem != null)
            {
                existingItem.Quantity += item.Quantity;
            }
            else
            {
                cart.Items.Add(item);
            }

            SaveCart(httpContext, cart);
        }
    }
}