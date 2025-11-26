using Microsoft.AspNetCore.Http;
using System.Collections.Generic;
using ECommerce.Application.ViewModels;

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
    }
}