using ECommerceWeb.MVC.Helpers;
using ECommerceWeb.MVC.Models;
using ECommerceWeb.MVC.Models.CartViewModels;
using ECommerceWeb.MVC.Models.FavoritesViewModels;
using Microsoft.AspNetCore.Mvc;

namespace ECommerceWeb.MVC.Controllers
{
    public class CartController : Controller
    {
        // View the cart page (GET request)
        public IActionResult Index()
        {
            var cart = SessionHelper.GetCart(HttpContext);
            ViewBag.Message = TempData["Message"]; // Message from redirection
            HttpContext.Session.SetInt32("CartCount", cart.Items.Sum(i => i.Quantity));
            return View(cart); // Return the actual cart page
        }

        // Add product to the cart (POST request)
        [HttpPost]
        public IActionResult AddProduct(int productId, string name, decimal price, int quantity = 1)
        {
            var cart = SessionHelper.GetCart(HttpContext);
            cart.AddItem(new CartItem { ProductId = productId, Name = name, Price = price, Quantity = quantity });

            SessionHelper.SaveCart(HttpContext, cart);

            // update cart count
            HttpContext.Session.SetInt32("CartCount", cart.Items.Sum(i => i.Quantity));

            TempData["Message"] = "Product added to the cart.";
            return RedirectToAction("Index");
        }

        // Edit product in the cart (POST request)
        [HttpPost]
        public IActionResult Edit(int productId, int quantity)
        {
            var cart = SessionHelper.GetCart(HttpContext);
            var item = cart.Items.FirstOrDefault(x => x.ProductId == productId);

            if (item != null)
            {
                if (quantity <= 0)
                    cart.Items.Remove(item);
                else
                    item.Quantity = quantity;
            }

            SessionHelper.SaveCart(HttpContext, cart);

            // update cart 
            HttpContext.Session.SetInt32("CartCount", cart.Items.Sum(i => i.Quantity));

            return RedirectToAction("Index");
        }

        // Clear the cart (POST request)
        [HttpPost]
        public IActionResult Clear()
        {
            var cart = new Cart();
            SessionHelper.SaveCart(HttpContext, cart);

            // update cart
            HttpContext.Session.SetInt32("CartCount", 0);

            TempData["Message"] = "Your cart has been cleared.";
            return RedirectToAction("Index");
        }

        //Favori listesinden cart a gitmek için
        [HttpPost]
        public IActionResult AddFavoritesToCart()
        {
            // Favorileri al
            var favorites = SessionHelper.GetFavorites(HttpContext) ?? new List<FavoriteItem>();
            var cart = SessionHelper.GetCart(HttpContext);

            foreach (var item in favorites)
            {
                // Sepete ekle (adet = 1)
                cart.AddItem(new CartItem
                {
                    ProductId = item.ProductId,
                    Name = item.Name,
                    Price = item.Price,
                    Quantity = 1
                });
            }

            SessionHelper.SaveCart(HttpContext, cart);

            // Sepet sayısını güncelle
            HttpContext.Session.SetInt32("CartCount", cart.Items.Sum(i => i.Quantity));

            TempData["Message"] = "All favorite items have been added to the cart.";

            return RedirectToAction("Index"); // Cart sayfasına yönlendir
        }
    }
}