using Newtonsoft.Json;
using Microsoft.AspNetCore.Http;
using System.Collections.Generic;
using ECommerceWeb.MVC.Models.FavoritesViewModels;
using ECommerceWeb.MVC.Models.CartViewModels;

namespace ECommerceWeb.MVC.Helpers
{
    // Bu metot, sepet ve favoriler için session yönetimini sağlar
    public static class SessionHelper
    {
        // Sepet için kullanılan session anahtarını tanımlıyoruz
        private const string SessionCartKey = "Cart";  // Anahtarı "Cart" olarak sabitliyoruz
        private const string SessionFavoritesKey = "Favorites"; // Favoriler için session anahtarı

        // Sepet session'ını almak (varsa)
        public static Cart GetCart(HttpContext httpContext)
        {
            var cart = httpContext.Session.GetString(SessionCartKey); // Sepet anahtarını buradan alıyoruz
            if (string.IsNullOrEmpty(cart))
                return new Cart(); // Eğer sepet boşsa yeni bir Cart döndür
            return JsonConvert.DeserializeObject<Cart>(cart); // Sepet bilgilerini deserialize ediyoruz
        }

        // Sepet session'ını kaydetmek (varsa)
        public static void SaveCart(HttpContext httpContext, Cart cart)
        {
            var serializedCart = JsonConvert.SerializeObject(cart); // Sepeti serialize ediyoruz
            httpContext.Session.SetString(SessionCartKey, serializedCart); // Sepeti session'a kaydediyoruz
        }

        // Sepet session'ını temizlemek
        public static void ClearCart(HttpContext httpContext)
        {
            httpContext.Session.Remove(SessionCartKey); // Sepet session'ını temizliyoruz
        }

        // Favoriler session'ını almak
        public static List<FavoriteItem> GetFavorites(HttpContext httpContext)
        {
            var favorites = httpContext.Session.GetString(SessionFavoritesKey); // Favoriler anahtarını buradan alıyoruz
            if (string.IsNullOrEmpty(favorites))
                return new List<FavoriteItem>(); // Eğer favoriler boşsa yeni bir liste döndür
            return JsonConvert.DeserializeObject<List<FavoriteItem>>(favorites); // Favoriler bilgilerini deserialize ediyoruz
        }

        // Favoriler session'ını kaydetmek
        public static void SaveFavorites(HttpContext httpContext, List<FavoriteItem> favorites)
        {
            var serializedFavorites = JsonConvert.SerializeObject(favorites); // Favorileri serialize ediyoruz
            httpContext.Session.SetString(SessionFavoritesKey, serializedFavorites); // Favorileri session'a kaydediyoruz
        }

        // Favoriler session'ını temizlemek (Eğer gereksizse)
        public static void ClearFavorites(HttpContext httpContext)
        {
            httpContext.Session.Remove(SessionFavoritesKey); // Favoriler session'ını temizliyoruz
        }
    }
}