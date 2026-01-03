using ECommerce.Application.DTOs.Product;
using ECommerce.Application.DTOs.ProductComment;
using ECommerce.Application.Interfaces.Services;
using ECommerce.Application.Services;
using ECommerce.Domain.Entities;
using ECommerce.Web.Mvc.Models.Category;
using ECommerce.Web.Mvc.Models.Comment;
using ECommerce.Web.Mvc.Models.Home;
using ECommerce.Web.Mvc.Models.Product;
using ECommerce.Web.MVC.Filters;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;

namespace ECommerce.Web.Mvc.Controllers
{
    public class ProductController : Controller
    {
        private readonly IProductService _productService;
        private readonly ICategoryService _categoryService;
       
     

        public ProductController(IProductService productService, ICategoryService categoryService)
        {
            _productService = productService;
            _categoryService = categoryService;
          
        }

        // Cookie Authentication içindeki NameIdentifier (UserId) claim'ini okur
        private int GetCurrentUserId() => int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);

        
        // ---------------- CREATE (GET) ----------------
        [Authorize(Roles = "Seller")] // Sadece satıcılar girebilir
        [ActiveUserAuthorize]
        public async Task<IActionResult> Create()
        {
            await PopulateCategoriesAsync();
            return View(new ProductCreateViewModel());
        }

        // ---------------- CREATE (POST) ----------------
        [HttpPost]
        [Authorize(Roles = "Seller")]
        [ValidateAntiForgeryToken]
        [ActiveUserAuthorize]
        public async Task<IActionResult> Create(ProductCreateViewModel model)
        {
            if (!ModelState.IsValid)
            {
                await PopulateCategoriesAsync();
                return View(model);
            }

            // ViewModel -> DTO
            var dto = new ProductDTO
            {
                Name = model.Name,
                CategoryId = model.CategoryId,
                Price = model.Price,
                OldPrice = model.OldPrice,
                StockAmount = model.StockAmount,
                Details = model.Details,
                CreatedAt = DateTime.Now,
                Enabled = true,
                SellerId = GetCurrentUserId(),
                MainImageUrl = model.ImageUrl

            };

            await _productService.AddAsync(dto);
            TempData["Success"] = "Product added successfully.";
            return RedirectToAction("MyProducts", "Profile");
        }
        // ---------------- EDIT (GET) ----------------
        [Authorize(Roles = "Seller")]
        [ActiveUserAuthorize]
        public async Task<IActionResult> Edit(int id)
        {
            var dto = await _productService.GetAsync(id);
            if (dto == null) return NotFound();

            // Güvenlik: Başkasının ürününü düzenleyemez
            if (dto.SellerId != GetCurrentUserId())
            {
                TempData["Error"] = "You are not authorized to edit this product.";
                return RedirectToAction("MyProducts", "Profile");
            }

            await PopulateCategoriesAsync();
            return View(MapToEditViewModel(dto)); // DTO alan sürüme gidiyor

           
        }

        // ---------------- EDIT (POST) ----------------
        [HttpPost]
        [Authorize(Roles = "Seller")]
        [ValidateAntiForgeryToken]
        [ActiveUserAuthorize]
        public async Task<IActionResult> Edit(ProductEditViewModel model)
        {
            var existingDto = await _productService.GetAsync(model.Id);
            if (existingDto == null) return NotFound();
            if (existingDto.SellerId != GetCurrentUserId()) return Forbid();

            if (!ModelState.IsValid)
            {
                await PopulateCategoriesAsync();
                return View(model);
            }
            // ViewModel -> DTO
            existingDto.Name = model.Name;
            existingDto.Price = model.Price;
            existingDto.OldPrice = model.OldPrice;
            existingDto.CategoryId = model.CategoryId;
            existingDto.StockAmount = model.StockAmount;
            existingDto.Details = model.Details;
            existingDto.MainImageUrl = model.ImageUrl;

            if (!string.IsNullOrEmpty(model.ImageUrl))
            {
                existingDto.MainImageUrl = model.ImageUrl;
            }

            await _productService.UpdateAsync(existingDto);
            TempData["Success"] = "Product updated successfully.";
            return RedirectToAction("MyProducts", "Profile");
        }

        // ---------------- DELETE ----------------
        [HttpPost]
        [Authorize(Roles = "Seller")]
        [ValidateAntiForgeryToken]
        [ActiveUserAuthorize]
        public async Task<IActionResult> Delete(int id)
        {
            var dto = await _productService.GetAsync(id);
            if (dto == null) return NotFound();
            if (dto.SellerId != GetCurrentUserId()) return Forbid();

            await _productService.DeleteAsync(id);
            TempData["Success"] = "Product deleted successfully.";
            return RedirectToAction("MyProducts", "Profile");
        }

        // ---------------- COMMENT ----------------
        [HttpPost]
        [Authorize(Roles = "Buyer,Seller")]
        [ValidateAntiForgeryToken]
        [ActiveUserAuthorize]
        public async Task<IActionResult> Comment(int productId, CommentViewModel model)
        {
            // Eğer kullanıcı adminse, yorum yapmasına izin verme
            if (User.IsInRole("Admin"))
            {
                TempData["Error"] = "Admins cannot comment on products.";
                return RedirectToAction("ProductDetail", "Home", new { id = productId });
            }

            if (!ModelState.IsValid)
            {
                TempData["Error"] = "Invalid comment data."; 
                return RedirectToAction("ProductDetail", "Home", new { id = productId });
            }

            // Ürün bilgilerini al
            var product = await _productService.GetAsync(productId);
            if (product == null)
            {
                TempData["Error"] = "Product not found.";
                return RedirectToAction("ProductDetail", "Home", new { id = productId });
            }

            
            // Yorum için gerekli bilgileri modele ekle
            model.UserName = User.Identity.Name;  // Oturum açmış kullanıcı adı
            model.ProductName = product.Name;     // Ürün adı, product modelinden alınır

            try
            {
                // ViewModel -> ProductCommentDTO
                var commentDto = new ProductCommentDTO
                {
                    ProductId = productId,
                    UserId = GetCurrentUserId(),
                    Text = model.Text,
                    StarCount = model.Rating,
                    IsConfirmed = false, // Admin onayına düşer
                    CreatedAt = DateTime.UtcNow
                };

                await _productService.AddCommentAsync(productId, commentDto);
                TempData["Success"] = "Comment submitted for approval.";

            }
            catch (InvalidOperationException ex)
            {
                // Satın almamışsa buraya düşecek
                TempData["Error"] = ex.Message;
            }
            catch (Exception)
            {
                TempData["Error"] = "An error occurred while saving your comment.";
            }
            return RedirectToAction("ProductDetail", "Home", new { id = productId });
        }

        // ---------------- HELPERS ----------------
        private async Task PopulateCategoriesAsync()
        {
            var categories = await _categoryService.GetAllAsync();

            ViewData["Categories"] = categories
                .Select(c => new SelectListItem { Value = c.Id.ToString(), Text = c.Name })
                .ToList();
        }

        private async Task SetCommonViewDataAsync()
        {

            ViewBag.IsLogged = User.Identity?.IsAuthenticated == true;
            ViewBag.Username = User.FindFirst(ClaimTypes.Name)?.Value;

            // Sepet sayısını hala session'dan okuyoruz (Çünkü hibrit yapımızda sayıyı session güncelliyor)
            ViewBag.CartCount = HttpContext.Session.GetInt32("CartCount") ?? 0;

            var categories = await _categoryService.GetAllAsync();
            ViewBag.Categories = categories.Select(c => new CategoryViewModel
            {
                Id = c.Id,
                Name = c.Name,
                IconCssClass = c.IconCssClass
            }).ToList();
        }

        // ---------------- MAPPING ----------------

        // DTO'yu Listeleme Modelimize Çevirir
        private ProductListingViewModel MapToListingViewModel(ProductDTO dto)
        {
            return new ProductListingViewModel
            {
                Id = dto.Id,
                Name = dto.Name,
                CategoryId = dto.CategoryId,
                Category = dto.CategoryName,
                ImageUrl = dto.MainImageUrl ?? "/img/product/default.jpg",
                Price = dto.Price,
                Rating = dto.Rating,
                ReviewCount = dto.ReviewCount,
                Enabled = dto.Enabled,
                OldPrice = dto.OldPrice,
                CreatedAt = dto.CreatedAt
            };
        }
        // DTO'yu Düzenleme Modelimize Çevirir
        private ProductEditViewModel MapToEditViewModel(ProductDTO dto)
        {
            return new ProductEditViewModel
            {
                Id = dto.Id,
                Name = dto.Name,
                Price = dto.Price,
                OldPrice = dto.OldPrice,
                CategoryId = dto.CategoryId,
                StockAmount = dto.StockAmount,
                Details = dto.Details,
                ImageUrl = dto.MainImageUrl
            };
        }

        // ---------------- TOGGLE STATUS ----------------
        [HttpPost]
        [Authorize(Roles = "Seller,Admin")]
        [ValidateAntiForgeryToken]
        [ActiveUserAuthorize]
        public async Task<IActionResult> ToggleStatus(int id)
        {
            var dto = await _productService.GetAsync(id);
            if (dto == null) return NotFound();

            // Güvenlik: Sadece ürünün sahibi durum değiştirebilir (ve Admin)
            if (!User.IsInRole("Admin") && dto.SellerId != GetCurrentUserId())
            {
                TempData["Error"] = "You are not authorized to change this status.";
                return RedirectToAction("MyProducts", "Profile");
            }

            await _productService.ToggleStatusAsync(id);
            TempData["Success"] = "Product status updated successfully.";
            return RedirectToAction("MyProducts", "Profile");
        }

        
        public async Task<IActionResult> Search(string query, string? category, byte? rating)
        {
            if (string.IsNullOrWhiteSpace(query))
                return RedirectToAction("Listing", "Home");

            await SetCommonViewDataAsync();

            // Servis artık List<ProductDTO> dönüyor
            var productDtos = await _productService.SearchProductsAsync(query, category ?? "", rating);

            var model = new ProductListingViewModel
            {
                // MapToListingViewModel artık ProductDTO alacak şekilde güncellendi
                Products = productDtos.Select(MapToListingViewModel).ToList(),
                Query = query,
                Category = category,
                Rating = rating.HasValue ? (double)rating.Value : 0
            };

            return View("~/Views/Home/Listing.cshtml", model);
        }


    }
}