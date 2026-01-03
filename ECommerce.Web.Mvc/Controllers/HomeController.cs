using ECommerce.Application.DTOs.Product;
using ECommerce.Application.Interfaces.Services;
using ECommerce.Domain.Entities;
using ECommerce.Web.Mvc.Models.Category;
using ECommerce.Web.Mvc.Models.Comment;
using ECommerce.Web.Mvc.Models.Home;
using ECommerce.Web.Mvc.Models.Product;
using ECommerce.Web.MVC.Filters;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Linq;
using System.Security.Claims;

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

        // --- COMMON VIEW DATA (Cookie Auth + Session) ---
        private async Task SetCommonViewDataAsync()
        {

            ViewBag.IsLogged = User.Identity?.IsAuthenticated == true;
            ViewBag.Username = User.FindFirst(ClaimTypes.Name)?.Value;

            // Sepet sayýsýný hala session'dan okuyoruz (Çünkü hibrit yapýmýzda sayýyý session güncelliyor)
            ViewBag.CartCount = HttpContext.Session.GetInt32("CartCount") ?? 0;

            var categories = await _categoryService.GetAllAsync();
            ViewBag.Categories = categories.Select(c => new CategoryViewModel
            {
                Id = c.Id,
                Name = c.Name,
                IconCssClass = c.IconCssClass
            }).ToList();
        }

        // --- INDEX ------
        [Route("/")]
        [Route("home")]
        public async Task<IActionResult> Index()
        {
            await SetCommonViewDataAsync();

            // Servisler artýk DTO dönüyor
            var popular = await _productService.GetFeaturedProductsAsync();
            var discounted = await _productService.GetDiscountedProductsAsync();
            var newArrivals = await _productService.GetNewArrivalProductsAsync();
            var allProducts = await _productService.GetAllAsync();

            var model = new HomePageViewModel
            {
                Featured = new FeaturedProductsViewModel
                {
                    PopularProducts = popular.Select(MapDtoToListingViewModel).ToList(),
                    DiscountedProducts = discounted.Select(MapDtoToListingViewModel).ToList(),
                    NewArrivalProducts = newArrivals.Select(MapDtoToListingViewModel).ToList()
                },
                Categories = ViewBag.Categories,
                Products = allProducts.OrderByDescending(p => p.CreatedAt).Take(12).Select(MapDtoToListingViewModel).ToList(),
                FeaturedProducts = allProducts.Where(p => p.IsFeatured).Select(MapDtoToListingViewModel).ToList()
            };

            return View(model);
        }


        // --- LISTING ---
        //Sadece enabled/aktif ürünler bulunuyor diðerleri gösterilmiyor--GetActiveProductsAsync();
        [Route("listing")]
        public async Task<IActionResult> Listing(int page = 1, string filter = "all-products", string? category = null)
        {
            await SetCommonViewDataAsync();
            int pageSize = 9;
            filter = string.IsNullOrEmpty(filter) ? "all-products" : filter.ToLower();

            // Tüm ürünleri asenkron çekip Queryable'a çeviriyoruz (veya servise filtreli talep atýlabilir)
            var productsData = await _productService.GetActiveProductsAsync();
            var query = productsData.AsQueryable();
           // var query = _productService.GetActiveQuery(); // IQueryable dönen metot--denenebilir

            if (!string.IsNullOrEmpty(category))
            {
                query = query.Where(p => p.CategoryName.Equals(category, StringComparison.OrdinalIgnoreCase));
            }

            // Filtreleme Switch Bloðu
            query = filter switch
            {
                "top-rated" => query.OrderByDescending(p => p.Rating),
                "on-sale" => query.Where(p => p.OldPrice > p.Price),
                "new-arrivals" => query.Where(p => p.CreatedAt >= DateTime.Now.AddDays(-30)).OrderByDescending(p => p.CreatedAt),
                _ => query.OrderByDescending(p => p.CreatedAt)
            };

            var totalProducts = query.Count();
            var products = query.Skip((page - 1) * pageSize).Take(pageSize).Select(MapDtoToListingViewModel).ToList();

            var model = new ProductListingViewModel
            {
                Products = products,
                CurrentPage = page,
                TotalPages = (int)Math.Ceiling((double)totalProducts / pageSize),
                Filter = filter,
                Category = category
            };

            return View(model);
        }

        // --- PRODUCT DETAIL ---
        //Tüm ürünler gösterilir ama disabled olanlar cart'a eklenemez.
        [Route("product-details/{id}")]
        public async Task<IActionResult> ProductDetail(int id)
        {
            
            await SetCommonViewDataAsync();

            var productDto = await _productService.GetAsync(id);
            if (productDto == null) return NotFound();

            var userIdClaim = User.FindFirstValue(ClaimTypes.NameIdentifier);
            bool canComment = false;

            if (!string.IsNullOrEmpty(userIdClaim))
            {
                // Artýk servis üzerinden tertemiz eriþiyoruz
                canComment = await _productService.HasUserPurchasedProductAsync(int.Parse(userIdClaim), id);
            }

            ViewBag.CanComment = canComment;
           

            var model = new ProductDetailViewModel
            {
                Id = productDto.Id,
                Name = productDto.Name,
                Category = productDto.CategoryName ?? "",
                Price = productDto.Price,
                OldPrice = productDto.OldPrice > productDto.Price ? productDto.OldPrice : null,
                ImageUrl = productDto.MainImageUrl ?? "/img/product/default.jpg",
                Description = productDto.Details ?? "",
                Availability = productDto.StockAmount > 0 ? "In Stock" : "Out of Stock",
                Enabled = productDto.Enabled,
                Rating = productDto.Comments != null && productDto.Comments.Any(c => c.IsConfirmed)
                ? (byte)productDto.Comments.Where(c => c.IsConfirmed).Average(c => c.StarCount)
                : (byte)0,
                ReviewCount = productDto.Comments?.Count(c => c.IsConfirmed) ?? 0,
                Comments = productDto.Comments?.Where(c => c.IsConfirmed).Select(c => new CommentViewModel
                {
                    Id = c.Id,
                    UserName = c.UserName ?? "Anonymous",
                    Text = c.Text,
                    Rating = c.StarCount,
                    IsApproved = c.IsConfirmed
                }).ToList() ?? new List<CommentViewModel>(),
                RelatedProducts = (await _productService.GetRelatedProductsAsync(productDto.CategoryId, productDto.Id))
                    .Select(MapDtoToProductListViewModel)
                    .ToList()
            };

            return View(model);
        }

        // --- STATIC PAGES ---
        [Route("about-us")]
        public async Task<IActionResult> AboutUs() { await SetCommonViewDataAsync(); return View(); }

        [Route("contact")]
        public async Task<IActionResult> Contact() { await SetCommonViewDataAsync(); return View(); }

        [Route("privacy-policy")]
        public async Task<IActionResult> PrivacyPolicy() { await SetCommonViewDataAsync(); return View();}


        // --- DTO'dan VIEWMODEL'e MAPPER'LAR ---
        private ProductListingViewModel MapDtoToListingViewModel(ProductDTO dto)
        {
            return new ProductListingViewModel
            {
                Id = dto.Id,
                Name = dto.Name,
                Category = dto.CategoryName,
                Price = dto.Price,
                OldPrice = dto.OldPrice,
                ImageUrl = dto.MainImageUrl ?? "/img/product/default.jpg",
                Rating = dto.Rating,
                ReviewCount = dto.ReviewCount,
                CreatedAt = dto.CreatedAt
            };
        }

        private ProductListViewModel MapDtoToProductListViewModel(ProductDTO dto)
        {
            return new ProductListViewModel
            {
                Id = dto.Id,
                Name = dto.Name,
                Price = dto.Price,
                OldPrice = dto.OldPrice > dto.Price ? dto.OldPrice : null,
                ImageUrl = dto.MainImageUrl ?? "/img/product/default.jpg",
                Rating = (byte?)dto.Rating,
                ReviewCount = dto.ReviewCount,
                IsActive = dto.Enabled
            };
        }

        // ---------------- FEATURED ----------------
        // Popüler, Ýndirimli ve Yeni ürünlerin bir arada gösterildiði özel sayfa
        //Bu kategorilerdeki disabled olanlar da gösterilir -- details e gidince aktif olmadýðý yazýyor-cart a eklenemez.
        [Route("featured")]
        public async Task<IActionResult> Featured()
        {
            await SetCommonViewDataAsync();

            // Tüm görevleri ayný anda baþlatarak bekleme süresini azaltabiliriz (Task.WhenAll opsiyoneldir)
            var popularTask = _productService.GetFeaturedProductsAsync();
            var discountedTask = _productService.GetDiscountedProductsAsync();
            var arrivalsTask = _productService.GetNewArrivalProductsAsync();

            await Task.WhenAll(popularTask, discountedTask, arrivalsTask);

            var model = new FeaturedProductsViewModel
            {
                PopularProducts = popularTask.Result.Select(MapDtoToListingViewModel).ToList(),
                DiscountedProducts = discountedTask.Result.Select(MapDtoToListingViewModel).ToList(),
                NewArrivalProducts = arrivalsTask.Result.Select(MapDtoToListingViewModel).ToList()
            };

            return View(model);
        }

    }
}
