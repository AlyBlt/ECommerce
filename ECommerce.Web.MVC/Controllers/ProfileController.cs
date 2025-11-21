using ECommerceWeb.MVC.Models.AuthViewModels;
using ECommerceWeb.MVC.Models.HomeViewModels;
using ECommerceWeb.MVC.Models.OrderviewModels;
using Microsoft.AspNetCore.Mvc;
using System.Text.Json;
using System.Diagnostics;

namespace ECommerceWeb.MVC.Controllers
{
    public class ProfileController : Controller
    {
        // Session keys
        private const string SessionUserKey = "UserInfo";
        private const string SessionOrderKey = "LastOrder"; // Last order JSON

        private bool CheckLogin() => HttpContext.Session.GetString(SessionUserKey) != null;

        private UserInformation GetUserFromSession()
        {
            var userJson = HttpContext.Session.GetString(SessionUserKey);
            return string.IsNullOrEmpty(userJson) ? null : JsonSerializer.Deserialize<UserInformation>(userJson);
        }

        private void SaveUserToSession(UserInformation user)
        {
            HttpContext.Session.SetString(SessionUserKey, JsonSerializer.Serialize(user));
        }

        private void SetUserToViewBag(UserInformation user)
        {
            if (user != null)
            {
                ViewBag.Username = user.FullName;
                ViewBag.Email = user.Email;
            }
        }

        private void SetUserToViewBag() => SetUserToViewBag(GetUserFromSession());

        // ---------------- DETAILS ----------------
        [Route("profile")]
        [Route("profile/details")]
        public IActionResult Details()
        {
            if (!CheckLogin()) return RedirectToAction("Login", "Auth");

            var user = GetUserFromSession();
            ViewBag.PageTitle = "My Profile";
            return View(user);
        }

        // ---------------- EDIT GET ----------------
        [Route("profile/edit")]
        public IActionResult Edit()
        {
            if (!CheckLogin()) return RedirectToAction("Login", "Auth");

            var user = GetUserFromSession();
            ViewBag.PageTitle = "Edit Profile";
            return View(user);
        }

        // ---------------- EDIT POST ----------------
        [HttpPost]
        [Route("profile/edit")]
        public IActionResult Edit(UserInformation model)
        {
            // Clear any previous ModelState errors
            ModelState.Clear(); //!!!  sadece “view’e eski değer gelmesin” diye kullandık denemek için, validation çözmek için değil.

            var existing = GetUserFromSession();
            if (existing == null)
                return RedirectToAction("Login", "Auth");

            // Keep email and password intact
            model.Email = existing.Email;
            model.Password = existing.Password;

            // Save updated data to session
            SaveUserToSession(model);

            TempData["Success"] = "Profile updated successfully.";

            return RedirectToAction("Details");
        }

        // ---------------- MY ORDERS ----------------
        [Route("profile/my-orders")]
        public IActionResult MyOrders()
        {
            if (!CheckLogin())
                return RedirectToAction("Login", "Auth");

            SetUserToViewBag();

            var orderJson = HttpContext.Session.GetString("Orders");
            List<Order> orders;

            if (string.IsNullOrEmpty(orderJson))
            {
                orders = new List<Order>();
                ViewBag.Empty = true;
            }
            else
            {
                orders = JsonSerializer.Deserialize<List<Order>>(orderJson);
                ViewBag.Empty = orders.Count == 0;
            }

            ViewBag.PageTitle = "My Orders";
            return View(orders);
        }

        // ---------------- MY PRODUCTS ----------------
        [Route("profile/my-products")]
        public IActionResult MyProducts()
        {
            if (!CheckLogin())
                return RedirectToAction("Login", "Auth");

            SetUserToViewBag();

            var products = new List<ProductListingModel>
            {
                new ProductListingModel { Id = 1, Name = "Organic Banana", ImageUrl="/img/product/product-1.jpg", Category="Fruits", Price=20, OldPrice=25 },
                new ProductListingModel { Id = 2, Name = "Fresh Milk", ImageUrl="/img/product/product-2.jpg", Category="Dairy", Price=18 },
                new ProductListingModel { Id = 3, Name = "Natural Honey", ImageUrl="/img/product/product-3.jpg", Category="Organic", Price=45 }
            };

            ViewBag.PageTitle = "My Products";
            return View(products);
        }
    }
}