using ECommerce.Application.Interfaces;
using ECommerce.Application.ViewModels;
using ECommerce.Data.Entities;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Linq;

namespace ECommerce.Web.Mvc.Controllers
{
    public class ProfileController : Controller
    {
        private const string SessionUserIdKey = "UserId";
        private readonly IUserService _userService;

        public ProfileController(IUserService userService)
        {
            _userService = userService;
        }

        // ---------------- SESSION & LOGIN ----------------
        private bool IsLoggedIn() => HttpContext.Session.GetInt32(SessionUserIdKey) != null;

        private UserEntity? GetUserFromDb()
        {
            var userId = HttpContext.Session.GetInt32(SessionUserIdKey);
            if (!userId.HasValue) return null;

            return _userService.Get(userId.Value, includeOrders: true, includeProducts: true);
        }

        // ---------------- DETAILS ----------------
        [Route("profile")]
        [Route("profile/details")]
        public IActionResult Details()
        {
            if (!IsLoggedIn()) return RedirectToAction("Login", "Auth");

            var user = GetUserFromDb();
            if (user == null) return RedirectToAction("Login", "Auth");

            var model = new UserDetailsViewModel
            {
                FirstName = user.FirstName,
                LastName = user.LastName,
                Email = user.Email,
                Phone = user.Phone,
                Address = user.Address,
                IsSellerApproved = user.IsSellerApproved
            };

            ViewBag.PageTitle = "My Profile";
            return View(model);
        }

        // ---------------- EDIT ----------------
        [Route("profile/edit")]
        [HttpGet]
        public IActionResult Edit()
        {
            if (!IsLoggedIn()) return RedirectToAction("Login", "Auth");

            var user = GetUserFromDb();
            if (user == null) return RedirectToAction("Login", "Auth");

            var model = new UserEditViewModel
            {
                FirstName = user.FirstName,
                LastName = user.LastName,
                Email = user.Email,
                Phone = user.Phone,
                Address = user.Address
            };

            ViewBag.PageTitle = "Edit Profile";
            return View(model);
        }

        [HttpPost]
        [Route("profile/edit")]
        public IActionResult Edit(UserEditViewModel model)
        {
            var user = GetUserFromDb();
            if (user == null) return RedirectToAction("Login", "Auth");

            user.FirstName = model.FirstName;
            user.LastName = model.LastName;

            // Email değişmiyor ama modelden geldiği için güncelliyoruz
            user.Email = model.Email;

            // EKLENMESİ GEREKENLER
            user.Phone = model.Phone;
            user.Address = model.Address;

            _userService.Update(user);

            TempData["Success"] = "Profile updated successfully.";
            return RedirectToAction("Details");
        }

        // ---------------- MY ORDERS ----------------
        [Route("profile/my-orders")]
        public IActionResult MyOrders()
        {
            if (!IsLoggedIn()) return RedirectToAction("Login", "Auth");

            var user = GetUserFromDb();
            if (user == null) return RedirectToAction("Login", "Auth");

            // -------------------- USER INFO → ViewBag.User --------------------
            var userVm = new UserInformationViewModel
            {
                FullName = $"{user.FirstName} {user.LastName}",
                Email = user.Email,
                Phone = user.Phone,
                Address = user.Address,
               
            };
            ViewBag.User = userVm;
            // -----------------------------------------------------------------

            // ---------------- ORDER LIST ----------------
            var orders = user.Orders?.Select(o => new OrderSummaryViewModel
            {
                Id = o.Id,
                OrderCode = o.OrderCode,
                CreatedAt = o.CreatedAt,
                PaymentMethod = o.PaymentMethod ?? "Credit Card",
                Items = o.OrderItems?.Select(oi => new OrderItemViewModel
                {
                    ProductName = oi.Product?.Name ?? "",
                    UnitPrice = oi.UnitPrice,
                    Quantity = (byte)oi.Quantity
                }).ToList() ?? new List<OrderItemViewModel>(),
                TotalAmount = o.OrderItems?.Sum(oi => oi.UnitPrice * oi.Quantity) ?? 0
            }).ToList() ?? new List<OrderSummaryViewModel>();

            ViewBag.Empty = !orders.Any();
            ViewBag.PageTitle = "My Orders";

            return View(orders);
        }

        // ---------------- MY PRODUCTS ----------------
        [Route("profile/my-products")]
        public IActionResult MyProducts()
        {
            if (!IsLoggedIn()) return RedirectToAction("Login", "Auth");

            var user = GetUserFromDb();
            if (user == null) return RedirectToAction("Login", "Auth");

            // ViewBag.User
            ViewBag.User = new UserInformationViewModel
            {
                FullName = $"{user.FirstName} {user.LastName}",
                Email = user.Email,
                Phone = user.Phone,
                Address = user.Address,
            };

            // ViewModel’e dönüştürme
            var products = user.Products?.Select(p => new ProductListingViewModel
            {
                Id = p.Id,
                Name = p.Name,
                Category = p.Category?.Name ?? "",
                ImageUrl = "/img/no-image.png", // çünkü entity'de resim yok
                Price = p.Price,
                InCart = false,
                Comments = p.Comments?.Select(c => new CommentViewModel
                {
                    Id = c.Id,
                    UserName = c.User?.FirstName ?? "User",
                    ProductName = p.Name,
                    Text = c.Text,
                    Rating = c.StarCount,
                    IsApproved = c.IsConfirmed
                }).ToList() ?? new List<CommentViewModel>()
            }).ToList() ?? new List<ProductListingViewModel>();

            ViewBag.PageTitle = "My Products";
            return View(products);
        }

        // ---------------- REQUEST SELLER ROLE ----------------
        [Route("profile/request-seller")]
        public IActionResult RequestSellerRole()
        {
            if (!IsLoggedIn()) return RedirectToAction("Login", "Auth");

            var user = GetUserFromDb();
            if (user == null) return RedirectToAction("Login", "Auth");

            if (!user.IsSellerApproved)
            {
                user.IsSellerApproved = false; // Talep kaydı için flag
                _userService.Update(user);
                TempData["Success"] = "Seller request submitted. Admin approval pending.";
            }
            else
            {
                TempData["Info"] = "You are already a seller.";
            }

            return RedirectToAction("Details");
        }
    }
}