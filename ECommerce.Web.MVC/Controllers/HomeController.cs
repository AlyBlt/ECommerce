using ECommerce.Application.Interfaces;
using ECommerce.Application.ViewModels;
using Microsoft.AspNetCore.Mvc;
using System.Linq;

namespace ECommerce.Web.Mvc.Controllers
{
    public class HomeController : Controller
    {
        private readonly IProductService _productService;
        private readonly ICategoryService _categoryService;

        public HomeController(IProductService productService, ICategoryService categoryService)
        {
            _productService = productService;
            _categoryService = categoryService;
        }

        // ---------------- SESSION HELPERS ----------------
        private void SetUserToViewBag()
        {
            ViewBag.IsLogged = HttpContext.Session.GetString("Username") != null;
            ViewBag.Username = HttpContext.Session.GetString("Username");
            ViewBag.CartCount = HttpContext.Session.GetInt32("CartCount") ?? 0;
        }

        // ---------------- HOME ----------------
        [Route("/")]
        [Route("home")]
        public IActionResult Index()
        {
            SetUserToViewBag();

            // FEATURED ÜRÜNLER
            var featuredProducts = new FeaturedProductsViewModel
            {
                PopularProducts = _productService.GetPopularProducts()
                                                 .Select(MapToListingViewModel)
                                                 .ToList(),

                DiscountedProducts = _productService.GetDiscountedProducts()
                                                    .Select(MapToListingViewModel)
                                                    .ToList(),

                NewArrivalProducts = _productService.GetNewArrivalProducts()
                                                    .Select(MapToListingViewModel)
                                                    .ToList()
            };

            // KATEGORÝLER
            var categories = _categoryService.GetAll()
                .Select(c => new CategoryViewModel
                {
                    Id = c.Id,
                    Name = c.Name,
                    IconCssClass = c.IconCssClass
                }).ToList();

            // ANA SAYFADA GÖSTERÝLECEK ÜRÜNLER (ör: latest)
            var products = _productService.GetAll()
                .OrderByDescending(p => p.CreatedAt)
                .Take(12)
                .Select(MapToListingViewModel)
                .ToList();

            var featuredSelectedProducts = _productService.GetAll()
                .Where(p => p.IsFeatured)
                .Select(MapToListingViewModel)
                .ToList();


            // HOME PAGE MODEL

            var model = new HomePageViewModel
            {
                Featured = featuredProducts,
                Categories = categories,
                Products = products,
                FeaturedProducts = featuredSelectedProducts
            };



            return View(model);
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

        // ---------------- LISTING ----------------
        [Route("listing")]
        public IActionResult Listing(string? filter)  // filter parametresi URL'den gelecek
        {
            SetUserToViewBag();

            // Eðer filter null veya boþ ise varsayýlan olarak "new-arrivals" seç
            filter = string.IsNullOrEmpty(filter) ? "new-arrivals" : filter.ToLower();  // Güvenli kontrol eklenmiþ

            IQueryable<ECommerce.Data.Entities.ProductEntity> productsQuery = _productService.GetAll().AsQueryable();

            // Filter parametresine göre ürünleri filtreliyoruz
            switch (filter)
            {
                case "top-rated":
                    productsQuery = productsQuery.Where(p => p.Comments != null && p.Comments.Any())  // Eðer yorumlar varsa
                                                 .OrderByDescending(p => p.Comments.Average(c => c.StarCount)); // Yorum ortalamasýna göre sýralama
                    break;

                case "on-sale":
                    productsQuery = productsQuery.Where(p => p.OldPrice > p.Price); // Ýndirimli ürünler
                    break;

                case "new-arrivals":
                    productsQuery = productsQuery.OrderByDescending(p => p.CreatedAt); // En yeni ürünler
                    break;

                default:
                    productsQuery = productsQuery.OrderByDescending(p => p.CreatedAt); // Varsayýlan olarak yeni gelen ürünleri listele
                    break;
            }

            // Filtrelenmiþ ürünleri al ve ProductListingViewModel'e dönüþtür
            var products = productsQuery
                            .Take(12) // Sayfalama yapýyorsanýz bu satýrý deðiþtirebilirsiniz
                            .Select(p => MapToListingViewModel(p)) // Mapleme
                            .ToList();

            // Kategorileri getir ve ViewBag'e ata
            ViewBag.Categories = _categoryService.GetAll()
                                                 .Select(c => new CategoryViewModel
                                                 {
                                                     Id = c.Id,
                                                     Name = c.Name,
                                                     IconCssClass = c.IconCssClass
                                                 }).ToList();

            return View(products);
        }

        // ---------------- DETAILS ----------------
        [Route("product/{id}/details")]
        public IActionResult ProductDetail(int id)
        {
            SetUserToViewBag();

            var product = _productService.Get(id);
            if (product == null) return NotFound();

            var model = new ProductDetailViewModel
            {
                Id = product.Id,
                Name = product.Name,
                Category = product.Category?.Name ?? "",
                Price = product.Price,
                OldPrice = product.OldPrice,
                ImageUrl = product.ImageUrl ?? "/img/product/default.jpg",
                Description = product.Details ?? "",
                Availability = product.StockAmount > 0 ? "In Stock" : "Out of Stock",
                Rating = product.Comments?.Any() == true ? product.Comments.Average(c => c.StarCount) : 0,
                ReviewCount = product.Comments?.Count() ?? 0,
                Comments = product.Comments?.Select(c => new CommentViewModel
                {
                    Id = c.Id,
                    UserName = c.User != null ? $"{c.User.FirstName} {c.User.LastName}" : "Anonymous",
                    ProductName = product.Name,
                    Text = c.Text,
                    Rating = c.StarCount,
                    IsApproved = c.IsConfirmed
                }).ToList() ?? new List<CommentViewModel>(),
                RelatedProducts = _productService.GetAll()
                                     .Where(p => p.Id != id)
                                     .Take(4)
                                     .Select(p => new ProductListingViewModel
                                     {
                                         Id = p.Id,
                                         Name = p.Name,
                                         Category = p.Category?.Name ?? "",
                                         Price = p.Price,
                                         OldPrice = p.OldPrice,
                                         ImageUrl = p.ImageUrl ?? "/img/product/default.jpg"
                                     }).ToList()
            };

            return View(model);
        }

        // ---------------- FEATURED ----------------
        // Featured ürünleri göstermek için
        [Route("featured")]
        public IActionResult Featured()
        {
            SetUserToViewBag();

            // Popüler ürünler - MapToListingViewModel ile dönüþtürme yapýyoruz
            var popularProducts = _productService.GetPopularProducts()
                                                 .Select(MapToListingViewModel)
                                                 .ToList();

            // Ýndirimli ürünler - MapToListingViewModel ile dönüþtürme yapýyoruz
            var discountedProducts = _productService.GetDiscountedProducts()
                                                     .Select(MapToListingViewModel)
                                                     .ToList();

            // Yeni gelen ürünler - MapToListingViewModel ile dönüþtürme yapýyoruz
            var newArrivalProducts = _productService.GetNewArrivalProducts()
                                                     .Select(MapToListingViewModel)
                                                     .ToList();

            // Model olarak view'a gönder
            var model = new FeaturedProductsViewModel
            {
                PopularProducts = popularProducts,
                DiscountedProducts = discountedProducts,
                NewArrivalProducts = newArrivalProducts
            };

            return View(model);  // Featured view'a gönder
        }



        // ---------------- HELPER: ENTITY -> LISTING VM ----------------
        private ProductListingViewModel MapToListingViewModel(ECommerce.Data.Entities.ProductEntity product)
        {
            return new ProductListingViewModel
            {
                Id = product.Id,
                Name = product.Name,
                Category = product.Category?.Name ?? "",
                Price = product.Price,
                OldPrice = product.OldPrice,
                ImageUrl = product.ImageUrl ?? "/img/product/default.jpg",

                // YENÝ: Rating ve ReviewCount dolduruluyor
                Rating = product.Comments?.Any() == true
                            ? product.Comments.Average(c => c.StarCount)
                            : 0, // Eðer yorum varsa, ortalama puaný al, yoksa 0

                ReviewCount = product.Comments?.Count() ?? 0, // Yorum sayýsýný al, yoksa 0

                CreatedAt = product.CreatedAt,
                InCart = false,

                // Yorumlarý aktar
                Comments = product.Comments?.Select(c => new CommentViewModel
                {
                    Id = c.Id,
                    UserName = c.User != null ? $"{c.User.FirstName} {c.User.LastName}" : "Anonymous",
                    ProductName = product.Name,
                    Text = c.Text,
                    Rating = c.StarCount,
                    IsApproved = c.IsConfirmed
                }).ToList() ?? new List<CommentViewModel>()
            };
        }



    }
}
