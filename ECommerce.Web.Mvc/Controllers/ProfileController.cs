using ECommerce.Application.DTOs.User;
using ECommerce.Application.Interfaces.Services;
using ECommerce.Web.Mvc.Models.Comment;
using ECommerce.Web.Mvc.Models.Home;
using ECommerce.Web.Mvc.Models.Order;
using ECommerce.Web.Mvc.Models.User;
using ECommerce.Web.MVC.Filters;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;

namespace ECommerce.Web.Mvc.Controllers
{
    [Authorize]
    [ActiveUserAuthorize]
    public class ProfileController : Controller
    {
       private readonly IUserService _userService;

        public ProfileController(IUserService userService)
        {
            _userService = userService;
        }

        // Cookie Authentication içindeki NameIdentifier (UserId) claim'ini okur
        private int GetCurrentUserId() => int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);

        // ---------------- DETAILS (GET) ----------------
        [Route("profile")]
        [Route("profile/details")]
        public async Task<IActionResult> Details()
        {
            var userDto = await _userService.GetAsync(GetCurrentUserId());
            if (userDto == null) return RedirectToAction("Login", "Auth");

            var model = new UserDetailsViewModel
            {
                FirstName = userDto.FirstName,
                LastName = userDto.LastName,
                Email = userDto.Email,
                Phone = userDto.Phone,
                Address = userDto.Address,
                IsSellerApproved = userDto.IsSellerApproved
            };

            ViewBag.PageTitle = "My Profile";
            return View(model);
        }

        // ---------------- EDIT (GET) ----------------
        [HttpGet]
        [Route("profile/edit")]
        public async Task<IActionResult> Edit()
        {
            var userDto = await _userService.GetAsync(GetCurrentUserId());
            if (userDto == null) return RedirectToAction("Login", "Auth");

            var model = new UserEditViewModel
            {
                FirstName = userDto.FirstName,
                LastName = userDto.LastName,
                Email = userDto.Email,
                Phone = userDto.Phone,
                Address = userDto.Address
            };

            ViewBag.PageTitle = "Edit Profile";
            return View(model);
        }

        // ---------------- EDIT (POST) ----------------
        [HttpPost]
        [Route("profile/edit")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(UserEditViewModel model)
        {
            if (!ModelState.IsValid) return View(model);

            var userDto = await _userService.GetAsync(GetCurrentUserId());
            if (userDto == null) return NotFound();

            userDto.FirstName = model.FirstName;
            userDto.LastName = model.LastName;
            userDto.Email = model.Email;
            userDto.Phone = model.Phone;
            userDto.Address = model.Address;

            await _userService.UpdateAsync(userDto);

            TempData["Success"] = "Profile updated successfully.";
            return RedirectToAction(nameof(Details));
        }

        // ---------------- MY ORDERS (GET) ----------------
        [Authorize(Roles = "Seller,Buyer")]
        [Route("profile/my-orders")]
        public async Task<IActionResult> MyOrders()
        {
            // Kullanıcıyı siparişleriyle birlikte getir (Service tarafında Include edilmeli)
            var userDto = await _userService.GetAsync(GetCurrentUserId(), includeOrders: true);
            if (userDto == null) return NotFound();

            ViewBag.User = new UserInformationViewModel
            {
                FullName = $"{userDto.FirstName} {userDto.LastName}",
                Email = userDto.Email,
                Phone = userDto.Phone,
                Address = userDto.Address
            };

            var orders = userDto.Orders?.Select(o => new OrderSummaryViewModel
            {
                Id = o.Id,
                OrderCode = o.OrderCode,
                CreatedAt = o.CreatedAt,
                PaymentMethod = o.PaymentMethod ?? "Credit Card",
                TotalAmount = o.TotalPrice,
                Items = o.Items?.Select(oi => new OrderItemViewModel
                {
                    ProductName = oi.ProductName ?? "Product",
                    UnitPrice = oi.UnitPrice,
                    Quantity = (byte)oi.Quantity
                }).ToList() ?? new List<OrderItemViewModel>()
            }).OrderByDescending(x => x.CreatedAt).ToList() ?? new List<OrderSummaryViewModel>();

            ViewBag.Empty = !orders.Any();
            ViewBag.PageTitle = "My Orders";

            return View(orders);
        }

        // ---------------- MY PRODUCTS (GET) ----------------
        [Authorize(Roles = "Seller")]
        [Route("profile/my-products")]
        public async Task<IActionResult> MyProducts()
        {
            // TempData'daki veriyi temizle
            TempData.Clear();
            var userDto = await _userService.GetAsync(GetCurrentUserId(), includeProducts: true);
            if (userDto == null || !userDto.IsSellerApproved) return Forbid();
                       

            ViewBag.User = new UserInformationViewModel
            {
                FullName = $"{userDto.FirstName} {userDto.LastName}",
                Email = userDto.Email,
                Address = userDto.Address
            };

            var products = userDto.Products?.Select(p => new ProductListingViewModel
            {
                Id = p.Id,
                Name = p.Name,
                Category = p.CategoryName ?? "General",
                ImageUrl = p.MainImageUrl ?? "/img/product/default.jpg",
                Price = p.Price,
                OldPrice = p.OldPrice,
                Enabled = p.Enabled,
                Comments = p.Comments?.Select(c => new CommentViewModel
                {
                    Id = c.Id,
                    UserName = c.UserName ?? "Anonymous",
                    Text = c.Text,
                    Rating = c.StarCount,
                    IsApproved = c.IsConfirmed
                }).ToList() ?? new List<CommentViewModel>()
            }).ToList() ?? new List<ProductListingViewModel>();

            
            ViewBag.PageTitle = "My Products";
            return View(products);
        }

        // ---------------- REQUEST SELLER ROLE (POST) ----------------
        [HttpGet]
        [Authorize(Roles = "Buyer")]
        [Route("profile/request-seller")]
        public async Task <IActionResult> RequestSellerRole()
        {
            var userDto = await _userService.GetAsync(GetCurrentUserId());
            if (userDto == null) return NotFound();
            ViewBag.IsSellerApproved = userDto.IsSellerApproved;
            ViewBag.HasPendingRequest = userDto.HasPendingSellerRequest;
            return View(); // View döndürülecek
        }

        [HttpPost]
        [Authorize(Roles = "Buyer")]
        [Route("profile/request-seller")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> RequestSellerRole(RequestSellerViewModel model)
        {
            var userDto = await _userService.GetAsync(GetCurrentUserId());
            if (userDto == null) return NotFound();

            if (!userDto.IsSellerApproved && !userDto.HasPendingSellerRequest)
            {
                await _userService.RequestSellerStatusAsync(userDto.Id);
                TempData["Success"] = "Seller request submitted. Admin approval pending.";
                return RedirectToAction(nameof(Details));
            }
            else
            {
                TempData["Info"] = "You already have a pending request.";
                return RedirectToAction(nameof(RequestSellerRole));
            }

        }
    }
}