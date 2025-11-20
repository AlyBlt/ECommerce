using ECommerceWeb.MVC.Models;
using Microsoft.AspNetCore.Mvc;
using System.Text.Json;

namespace ECommerceWeb.MVC.Controllers
{
    public class OrderController : Controller
    {
        // Session anahtarları
        private const string SessionCartKey = "CartSession";  // Sepet bilgileri için
        private const string SessionOrderKey = "LastOrder";  // Son sipariş bilgisi için

        // Sepeti session'dan alır
        private Cart GetCart()
        {
            var json = HttpContext.Session.GetString(SessionCartKey);  // Session'dan json stringi al
            if (string.IsNullOrEmpty(json))
                return new Cart();  // Sepet yoksa boş sepet döndür

            // JSON'u Cart nesnesine dönüştür
            return JsonSerializer.Deserialize<Cart>(json);
        }

        // Kullanıcı bilgileri sayfası
        [HttpGet]
        public IActionResult Create()
        {
            // Sepeti kontrol et
            var cartJson = HttpContext.Session.GetString("CartSession");
            var cart = string.IsNullOrEmpty(cartJson) ? new Cart() : JsonSerializer.Deserialize<Cart>(cartJson);

            if (cart.Items.Count == 0)
            {
                TempData["Error"] = "Sepetiniz boş. Ödeme sayfasına geçemezsiniz!";
                return RedirectToAction("Test", "Cart"); // Sepet sayfasına yönlendir
            }


            return View(new UserInformation());
        }

        [HttpPost]
        public IActionResult Create(UserInformation model)
        {
            // Form doğrulaması başarısızsa tekrar göster
            if (!ModelState.IsValid)
                return View(model);

            // Kullanıcı bilgilerini session'a kaydet (JSON formatında)
            HttpContext.Session.SetString("UserInfo", JsonSerializer.Serialize(model));

            // Ödeme sayfasına yönlendir
            return RedirectToAction("Payment");
        }

        // Ödeme yöntemi seçimi
        [HttpGet]
        public IActionResult Payment()
        {
            // Boş PaymentInformation modeli ile ödeme formunu göster
            return View(new PaymentInformation());
        }

        [HttpPost]
        public IActionResult Payment(PaymentInformation model)
        {
            // Ödeme yöntemi seçilmemişse hata mesajı göster
            if (model.PaymentMethod == null)
            {
                ModelState.AddModelError("", "Ödeme yöntemi seçmelisiniz.");
                return View(model);
            }

            // Kullanıcı bilgilerini session'dan al
            var userJson = HttpContext.Session.GetString("UserInfo");
            var userInfo = JsonSerializer.Deserialize<UserInformation>(userJson);

            // Sepet bilgilerini al
            var cart = GetCart();

            // Sipariş nesnesini oluştur
            var order = new Order
            {
                Id = new Random().Next(100000, 999999), // rastgele sipariş numarası
                UserInformation = userInfo,
                Items = cart.Items.Select(x => new OrderItem
                {
                    ProductId = x.ProductId,
                    Name = x.Name,
                    Price = x.Price,
                    Quantity = x.Quantity
                }).ToList(),
                PaymentMethod = model.PaymentMethod
            };

            // Siparişi session'a kaydet
            HttpContext.Session.SetString(SessionOrderKey, JsonSerializer.Serialize(order));

            // Sepeti temizle (sipariş oluşturulduktan sonra)
            cart.Clear();
            HttpContext.Session.SetString(SessionCartKey, JsonSerializer.Serialize(cart));

            // Sipariş detayları sayfasına yönlendir
            return RedirectToAction("Details");
        }

        // Sipariş Detayları sayfası
        [HttpGet]
        public IActionResult Details()
        {
            // Son siparişi session'dan al
            var json = HttpContext.Session.GetString(SessionOrderKey);
            // Eğer sipariş yoksa ana sayfaya yönlendir
            if (json == null)
                return RedirectToAction("Index", "Home");

            // JSON'u Order nesnesine dönüştür ve view'a gönder
            var order = JsonSerializer.Deserialize<Order>(json);
            return View(order);
        }
    }
}