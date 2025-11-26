using ECommerce.Application.Interfaces;
using ECommerce.Application.ViewModels;
using ECommerce.Data.Entities;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;

namespace ECommerce.Web.Mvc.Controllers
{
    public class ProductController : Controller
    {
        private readonly IProductService _productService;
        private const string AdminSessionKey = "IsAdmin";

        public ProductController(IProductService productService)
        {
            _productService = productService;
        }

        // ---------------- SESSION HELPERS ----------------
        private bool IsAdmin() => HttpContext.Session.GetInt32(AdminSessionKey) == 1;

        private int? GetCurrentUserId()
        {
            const string SessionUserIdKey = "UserId";
            return HttpContext.Session.GetInt32(SessionUserIdKey);
        }

        // ---------------- HELPER: ENTITY → VIEWMODEL ----------------
        private ProductListingViewModel MapToViewModel(ProductEntity product)
        {
            return new ProductListingViewModel
            {
                Id = product.Id,
                Name = product.Name,
                Category = product.Category?.Name ?? "",
                Price = product.Price,
                OldPrice = null,
                ImageUrl = "", // DB’de yoksa placeholder
                Comments = product.Comments?.Select(c => new CommentViewModel
                {
                    Id = c.Id,
                    UserName = c.User != null ? $"{c.User.FirstName} {c.User.LastName}" : "Anonymous",
                    ProductName = product.Name,
                    Text = c.Text,
                    Rating = c.StarCount,
                    IsApproved = c.IsConfirmed
                }).ToList() ?? new List<CommentViewModel>(),
                InCart = false // Session cart’a göre güncellenebilir
            };
        }

        // ---------------- LISTING ----------------
        public IActionResult Listing()
        {
            var products = _productService.GetAll()
                .Select(MapToViewModel)
                .ToList();

            return View(products);
        }

        // ---------------- DETAILS ----------------
        public IActionResult Details(int id)
        {
            var product = _productService.Get(id);
            if (product == null) return NotFound();

            var model = MapToViewModel(product);
            return View(model);
        }

        // ---------------- CREATE ----------------
        public IActionResult Create()
        {
            if (!IsAdmin()) return Forbid();
            return View();
        }

        [HttpPost]
        public IActionResult Create(ProductListingViewModel model)
        {
            if (!IsAdmin()) return Forbid();
            if (!ModelState.IsValid) return View(model);

            var entity = new ProductEntity
            {
                Name = model.Name,
                CategoryId = 1, // Sabit kategori örneği
                Price = model.Price,
                CreatedAt = DateTime.Now,
                Enabled = true
            };

            _productService.Add(entity);
            return RedirectToAction("Listing");
        }

        // ---------------- EDIT ----------------
        public IActionResult Edit(int id)
        {
            if (!IsAdmin()) return Forbid();
            var entity = _productService.Get(id);
            if (entity == null) return NotFound();

            var model = MapToViewModel(entity);
            return View(model);
        }

        [HttpPost]
        public IActionResult Edit(ProductListingViewModel model)
        {
            if (!IsAdmin()) return Forbid();
            if (!ModelState.IsValid) return View(model);

            var entity = _productService.Get(model.Id);
            if (entity == null) return NotFound();

            entity.Name = model.Name;
            entity.Price = model.Price;

            _productService.Update(entity);
            return RedirectToAction("Listing");
        }

        // ---------------- DELETE ----------------
        public IActionResult Delete(int id)
        {
            if (!IsAdmin()) return Forbid();
            _productService.Delete(id);
            return RedirectToAction("Listing");
        }

        // ---------------- TOGGLE STATUS ----------------
        public IActionResult ToggleStatus(int id)
        {
            if (!IsAdmin()) return Forbid();
            _productService.ToggleStatus(id);
            return RedirectToAction("Listing");
        }

        // ---------------- ADD COMMENT ----------------
        [HttpPost]
        public IActionResult Comment(int productId, CommentViewModel model)
        {
            var userId = GetCurrentUserId();
            if (!userId.HasValue)
                return RedirectToAction("Login", "Auth");

            var product = _productService.Get(productId);
            if (product == null)
                return NotFound();

            var commentEntity = new ProductCommentEntity
            {
                ProductId = productId,
                UserId = userId.Value,
                Text = model.Text,
                StarCount = model.Rating,
                IsConfirmed = false, // Admin onayı bekler
                CreatedAt = DateTime.Now
            };

            if (product.Comments == null)
                product.Comments = new List<ProductCommentEntity>();

            product.Comments.Add(commentEntity);
            _productService.Update(product);

            return RedirectToAction("Details", new { id = productId });
        }
    }
}