using ECommerceWeb.MVC.Models;
using System.Text.Json;

namespace ECommerceWeb.MVC.Helpers
{
    //Bu metot veritabanı kullanımına kadar order ları tutmak için yapıldı. Order ve Cart Controllerlarda kullanılıyor.
    public static class SessionHelper
    {
        private const string SessionCartKey = "CartSession";

        public static Cart GetCart(HttpContext context)
        {
            var cartJson = context.Session.GetString(SessionCartKey);
            if (cartJson != null)
            {
                var cart = JsonSerializer.Deserialize<Cart>(cartJson);
                if (cart.Items == null) cart.Items = new List<CartItem>();
                return cart;
            }
            return new Cart(); // boş liste ile dön
        }

        public static void SaveCart(HttpContext context, Cart cart)
        {
            var cartJson = JsonSerializer.Serialize(cart);
            context.Session.SetString(SessionCartKey, cartJson);
        }

        public static void ClearCart(HttpContext context)
        {
            context.Session.Remove(SessionCartKey);
        }
    }
}
