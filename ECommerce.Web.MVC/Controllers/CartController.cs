using ECommerceWeb.MVC.Models;
using ECommerceWeb.MVC.Helpers;
using Microsoft.AspNetCore.Mvc;

namespace ECommerceWeb.MVC.Controllers
{
    public class CartController : Controller
    {
        // Sepete ürün eklemek için POST isteği
        [HttpPost]
        public IActionResult AddProduct(int productId, string name, decimal price, int quantity = 1)
        {
            var cart = SessionHelper.GetCart(HttpContext);
            cart.AddItem(new CartItem { ProductId = productId, Name = name, Price = price, Quantity = quantity });

            // Sepeti session'a kaydediyoruz
            SessionHelper.SaveCart(HttpContext, cart);

            // Kullanıcıyı bir sonraki sayfaya yönlendiriyoruz
            TempData["Message"] = "Ürün sepete eklendi."; // Yönlendirme sonrası mesaj taşıma
            return RedirectToAction("Test", "Cart"); // Sepet sayfasına yönlendirme
        }

        // Sepetteki bir ürünü düzenlemek için POST isteği
        [HttpPost]
        public IActionResult Edit(int productId, int quantity)
        {
            var cart = SessionHelper.GetCart(HttpContext);
            var item = cart.Items.FirstOrDefault(x => x.ProductId == productId);

            if (item != null)
            {
                if (quantity <= 0) // Eğer miktar sıfır veya daha küçükse ürünü sepetten çıkar
                    cart.Items.Remove(item);
                else
                    item.Quantity = quantity; // Miktarı güncelle
            }

            // Güncellenmiş sepeti session'a kaydediyoruz
            SessionHelper.SaveCart(HttpContext, cart);

            // Sepet sayfasına yönlendiriyoruz
            return View("Test", "Cart"); // Sepet sayfasına yönlendirme
        }

        // Sepet sayfasını görüntülemek için GET isteği
        public IActionResult Index()
        {
            var cart = SessionHelper.GetCart(HttpContext);
            ViewBag.Message = TempData["Message"]; // Yönlendirme sonrası gelen mesaj
            return View(cart); // Sepet görünümünü döndürüyoruz
        }

        [HttpPost]
        public IActionResult Clear()
        {
            var cart = new Cart();
            SessionHelper.SaveCart(HttpContext, cart); // Sepeti temizle
            TempData["Message"] = "Sepetiniz temizlendi.";
            return RedirectToAction("Test");
        }

        // Sepet sayfasına yönlendiren yardımcı metod
        public IActionResult Test()
        {
            var cart = SessionHelper.GetCart(HttpContext);
            return View(cart); // Test sayfasını döndürüyoruz
        }
    }
}