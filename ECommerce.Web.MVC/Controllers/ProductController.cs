using ECommerceWeb.MVC.Models.HomeViewModels;
using Microsoft.AspNetCore.Mvc;
using ECommerceWeb.MVC.Helpers;
using ECommerceWeb.MVC.Models.ProductViewModels;
using System.Linq;

namespace ECommerceWeb.MVC.Controllers
{
    public class ProductController : Controller
    {
        // Session üzerinden ürün listesi
        private List<ProductListingModel> GetProducts()
        {
            var products = HttpContext.Session.GetObjectFromJson<List<ProductListingModel>>("Products");
            if (products == null)
            {
                products = new List<ProductListingModel>
                {
                    new ProductListingModel { Id = 1, Name = "Crab Pool Security", Category="Dried Fruit", Price=30, ImageUrl="/img/product/product-1.jpg", Comments = new List<ProductComment>() },
                    new ProductListingModel { Id = 2, Name = "Vegetables’ Package", Category="Vegetables", Price=30, ImageUrl="/img/product/product-2.jpg", Comments = new List<ProductComment>() },
                    new ProductListingModel { Id = 3, Name = "Mixed Fruits", Category="Dried Fruit", Price=30, ImageUrl="/img/product/product-3.jpg", Comments = new List<ProductComment>() }
                };
                HttpContext.Session.SetObjectAsJson("Products", products);
            }
            return products;
        }

        private void SaveProducts(List<ProductListingModel> products)
        {
            HttpContext.Session.SetObjectAsJson("Products", products);
        }

        // Ürün oluşturma
        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        public IActionResult Create(ProductListingModel model)
        {
            if (!ModelState.IsValid) return View(model);

            var products = GetProducts();
            model.Id = products.Any() ? products.Max(p => p.Id) + 1 : 1;
            model.Comments = new List<ProductComment>();
            products.Add(model);
            SaveProducts(products);

            return RedirectToAction("Listing", "Home");
        }

        // Ürün düzenleme
        public IActionResult Edit(int id)
        {
            var product = GetProducts().FirstOrDefault(p => p.Id == id);
            if (product == null) return NotFound();
            return View(product);
        }

        [HttpPost]
        public IActionResult Edit(ProductListingModel model)
        {
            if (!ModelState.IsValid) return View(model);

            var products = GetProducts();
            var product = products.FirstOrDefault(p => p.Id == model.Id);
            if (product == null) return NotFound();

            product.Name = model.Name;
            product.Category = model.Category;
            product.Price = model.Price;
            product.OldPrice = model.OldPrice;
            product.ImageUrl = model.ImageUrl;

            SaveProducts(products);
            return RedirectToAction("Listing", "Home");
        }

        // Ürün silme
        public IActionResult Delete(int id)
        {
            var products = GetProducts();
            var product = products.FirstOrDefault(p => p.Id == id);
            if (product != null)
            {
                products.Remove(product);
                SaveProducts(products);
            }
            return RedirectToAction("Listing", "Home");
        }

        // Ürüne yorum ekleme ve yıldız
        [HttpPost]
        public IActionResult Comment(int productId, string userName, string commentText, int rating)
        {
            var products = GetProducts();
            var product = products.FirstOrDefault(p => p.Id == productId);
            if (product == null) return NotFound();

            product.Comments.Add(new ProductComment
            {
                UserName = userName,
                CommentText = commentText,
                Rating = rating,
                CreatedAt = DateTime.Now
            });

            SaveProducts(products);

            // ProductDetail HomeController’da olduğu için
            return RedirectToAction("ProductDetail", "Home", new { id = productId });
        }

        // Sepete ekleme
        [HttpPost]
        public IActionResult AddToCart(int id)
        {
            var cart = HttpContext.Session.GetObjectFromJson<List<int>>("Cart") ?? new List<int>();
            cart.Add(id);
            HttpContext.Session.SetObjectAsJson("Cart", cart);

            TempData["Message"] = "Product added to cart!";
            return RedirectToAction("Listing", "Home");
        }
    }
}