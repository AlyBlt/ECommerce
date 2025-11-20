using ECommerceWeb.MVC.Models;
using ECommerceWeb.MVC.Models.HomeViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Win32;
using System.Diagnostics;

namespace ECommerce.Web.MVC.Controllers
{
    //GENEL NOT (ALÝYE): Veritabaný olmadýðý için henüz, Session üzerinden demo/test amaçlý yapýldý.
    //O nedenle bazý kýsýmlar revize edildi. Ayrýca model üzerinden olduðu için modeller oluþturuldu.

    //Session bilgisini her view'a taþýmak için SetUserToViewBag() ekledik
    //_Header.cshtml dinamik hale gelir
    //Login- Username görünür
    //Logout- Login/Register görünür
    public class HomeController : Controller
    {
        [Route("/")] // home yazýlmadan sadece site adý ile açýlsýn diye yapýldý.
        [Route("home")] // .../home diyerek sayfaya yönlendirilmesi için yapýldý.
        public IActionResult Index()
        {
            SetUserToViewBag();
            return View();
        }

        [Route("about-us")]
        public IActionResult AboutUs()
        {
            SetUserToViewBag();
            return View(); 
        }

        [Route("contact")]
        public IActionResult Contact()
        {
            SetUserToViewBag();
            return View();
        }

        [Route("listing")]
        public IActionResult Listing()
        {
            // Cart count is taken from session
            ViewBag.CartCount = HttpContext.Session.GetInt32("CartCount") ?? 0;

            // Product list (static demo, can be changed with DB)
            var products = new List<ProductListingModel>
            {
                new ProductListingModel { Id = 1, Name = "Crab Pool Security", Category = "Dried Fruit", ImageUrl="/img/product/product-1.jpg", Price=30, OldPrice=36 },
                new ProductListingModel { Id = 2, Name = "Vegetables’ Package", Category = "Vegetables", ImageUrl="/img/product/product-2.jpg", Price=30 },
                new ProductListingModel { Id = 3, Name = "Mixed Fruits", Category = "Dried Fruit", ImageUrl="/img/product/product-3.jpg", Price=30, OldPrice=36 }
            };

            return View(products);

        }

        // Pages altýnda ki shop details sayfasýna yönlendiriliyor.
        //[Route("product/{categoryName}-{title}-{id}/details")]  Veritabaný baðlandýktan sonra route açýlýcak test sürecinde sayfa yok olarak gösteriyor.
        // SEO Friendly Route: /product/{categoryName}-{title}-{id}/details
        //Dinamik Model
        [Route("product/{categoryName}-{title}-{id}/details")]
        public IActionResult ProductDetail([FromRoute] string categoryName, [FromRoute] string title, [FromRoute] int id)
        {
            SetUserToViewBag();

            // Demo product info
            var product = new ProductDetailViewModel
            {
                Id = id,
                Name = title,
                Category = categoryName, // now exists in the model
                Price = 50,
                OldPrice = 60,
                Rating = 4.5,
                ReviewCount = 18,
                Description = "This is a demo product description. Will be replaced by DB data later.",
                Information = "Demo information text. Will change after DB integration.",
                Weight = 0.5,
                Availability = "In Stock",
                Shipping = "01 day shipping. Free pickup today",
                MainImageUrl = "/img/product/details/product-details-1.jpg",
                GalleryImages = new List<string>
            {
            "/img/product/details/product-details-1.jpg",
            "/img/product/details/product-details-2.jpg",
            "/img/product/details/product-details-3.jpg",
            "/img/product/details/product-details-4.jpg"
            },
                RelatedProducts = new List<ProductListingModel>
            {
            new ProductListingModel { Id = 4, Name="Crab Pool Security", Price=30, ImageUrl="/images/product/product-1.jpg"},
            new ProductListingModel { Id = 5, Name="Crab Pool Security", Price=30, ImageUrl="/images/product/product-2.jpg"},
            new ProductListingModel { Id = 6, Name="Crab Pool Security", Price=30, ImageUrl="/images/product/product-3.jpg"},
            new ProductListingModel { Id = 7, Name="Crab Pool Security", Price=30, ImageUrl="/images/product/product-7.jpg"}
            }
            };

            return View(product); // ProductDetail.cshtml
        }

        // Kullanýcý login ise username bilgisini header için ViewBag'e aktarýyoruz
        private void SetUserToViewBag()
        {
            ViewBag.IsLogged = HttpContext.Session.GetString("Username") != null;
            ViewBag.Username = HttpContext.Session.GetString("Username");
        }
    }
}
