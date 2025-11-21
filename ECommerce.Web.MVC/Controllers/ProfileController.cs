using ECommerceWeb.MVC.Models;
using ECommerceWeb.MVC.Models.HomeViewModels;
using Microsoft.AspNetCore.Mvc;
using System.Text.Json;

namespace ECommerceWeb.MVC.Controllers
{
    public class ProfileController : Controller
    {
        // Session key tanımları: Session'da tutmak istediğimiz verilerin anahtar isimleri
        // -------------------------------------------------------------------
        private const string SessionUserKey = "UserInfo";   // UserInformation modelinin JSON hali
        private const string SessionOrderKey = "LastOrder"; // Son sipariş JSON verisi

        // HEADER ÜSTÜ KULLANICI BİLGİLERİ
        // Amaç:
        // Kullanıcının giriş yapıp yapmadığını anlamak
        // Header’da (layout'ta) kullanıcı adı ve email'in dinamik görünmesini sağlamak
        // -------------------------------------------------------------------
        private void SetUserToViewBag()
        {
            var userJson = HttpContext.Session.GetString(SessionUserKey);

            // Eğer kullanıcı giriş yapmışsa Session içerisinde UserInformation JSON bulunur
            if (!string.IsNullOrEmpty(userJson))
            {
                // Session'daki JSON verisini UserInformation modeline çeviriyoruz
                var user = JsonSerializer.Deserialize<UserInformation>(userJson);

                // ViewBag ile tüm view’lara kullanıcı bilgisi taşıyoruz
                ViewBag.Username = user.FullName;
                ViewBag.Email = user.Email;
            }
            else
            {
                // Giriş yapılmamışsa header'da login/register görünür
                ViewBag.Username = null;
                ViewBag.Email = null;
            }
        }


        // LOGIN KONTROLÜ
        // Amaç:
        // Kullanıcı giriş yapmamışsa Profile sayfalarına erişimi engellemek
        // -------------------------------------------------------------------
        private bool CheckLogin()
        {
            return HttpContext.Session.GetString(SessionUserKey) != null;
        }


        //  PROFILE → DETAILS (Kullanıcı bilgilerini görüntüleme)
        // -------------------------------------------------------------------
        [Route("profile")]
        [Route("profile/details")]
        public IActionResult Details()
        {
            // Eğer kullanıcı giriş yapmamışsa login sayfasına yönlendiriyoruz
            if (!CheckLogin())
                return RedirectToAction("Login", "Auth");

            SetUserToViewBag();

            // Session’daki JSON veriyi alıp modele dönüştürüyoruz
            var userJson = HttpContext.Session.GetString(SessionUserKey);
            var user = JsonSerializer.Deserialize<UserInformation>(userJson);

            // Modeli view’a gönderiyoruz
            return View(user);
        }

        //  PROFILE → EDIT (GET) — profil bilgileri düzenleme formu
        //-------------------------------------------------------------------
        [Route("profile/edit")]
        public IActionResult Edit()
        {
            if (!CheckLogin())
                return RedirectToAction("Login", "Auth");

            SetUserToViewBag();

            // Formun default olarak kullanıcı bilgileriyle dolu gelmesi için
            var userJson = HttpContext.Session.GetString(SessionUserKey);
            var user = JsonSerializer.Deserialize<UserInformation>(userJson);

            return View(user);
        }


        //  PROFILE → EDIT (POST) — düzenlenen bilgileri kaydetme
        // -------------------------------------------------------------------
        [HttpPost]
        [Route("profile/edit")]
        public IActionResult Edit(UserInformation model)
        {
            // Eğer validation hatası varsa aynı formu hata mesajlarıyla tekrar gösterir
            if (!ModelState.IsValid)
            {
                SetUserToViewBag();
                return View(model);
            }

            // Kullanıcı bilgilerini Session’a JSON olarak kaydeder
            HttpContext.Session.SetString(SessionUserKey, JsonSerializer.Serialize(model));

            // Başarılı güncelleme mesajı
            TempData["Success"] = "Profile updated successfully.";

            return RedirectToAction("Details");
        }

        //  PROFILE → MY ORDERS — kullanıcı sipariş geçmişi
        // -------------------------------------------------------------------
        [Route("profile/my-orders")]
        public IActionResult MyOrders()
        {
            if (!CheckLogin())
                return RedirectToAction("Login", "Auth");

            SetUserToViewBag();

            // Son siparişi Session’dan alıyoruz
            var orderJson = HttpContext.Session.GetString(SessionOrderKey);

            // Hiç sipariş yoksa sayfada mesaj gösterebilmek için ViewBag kullanıyoruz
            if (string.IsNullOrEmpty(orderJson))
            {
                ViewBag.Empty = true;
                return View();
            }

            // Sipariş bulunduysa modele dönüştürüp view’e gönderiyoruz
            var order = JsonSerializer.Deserialize<Order>(orderJson);
            return View(order);
        }

        //  PROFILE → MY PRODUCTS — satıcı ürünleri (DEMO veri)
        // -------------------------------------------------------------------
        [Route("profile/my-products")]
        public IActionResult MyProducts()
        {
            if (!CheckLogin())
                return RedirectToAction("Login", "Auth");

            SetUserToViewBag();

            // Demo ürün listesi — veritabanı olmadığı için statik data kullandık
            var products = new List<ProductListingModel>
            {
                new ProductListingModel { Id = 1, Name = "Organic Banana", ImageUrl="/img/product/product-1.jpg", Category="Fruits", Price=20, OldPrice=25 },
                new ProductListingModel { Id = 2, Name = "Fresh Milk", ImageUrl="/img/product/product-2.jpg", Category="Dairy", Price=18 },
                new ProductListingModel { Id = 3, Name = "Natural Honey", ImageUrl="/img/product/product-3.jpg", Category="Organic", Price=45 }
            };

            return View(products);
        }
    }
}
