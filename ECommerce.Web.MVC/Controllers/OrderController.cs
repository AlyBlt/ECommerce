using ECommerceWeb.MVC.Models.CartViewModels;
using ECommerceWeb.MVC.Models.OrderviewModels;
using Microsoft.AspNetCore.Mvc;
using System.Text.Json;

namespace ECommerceWeb.MVC.Controllers
{
    public class OrderController : Controller
    {
        // Session keys
        private const string SessionCartKey = "CartSession";  // For cart information
        private const string SessionOrderKey = "LastOrder";  // For last order information

        // Retrieve cart from session
        private Cart GetCart()
        {
            var json = HttpContext.Session.GetString(SessionCartKey);  // Get JSON string from session
            if (string.IsNullOrEmpty(json))
                return new Cart();  // Return an empty cart if session is empty

            // Deserialize the JSON into a Cart object
            return JsonSerializer.Deserialize<Cart>(json);
        }

        // User information page
        [HttpGet]
        public IActionResult Create()
        {
            // Check the cart
            var cartJson = HttpContext.Session.GetString("CartSession");
            var cart = string.IsNullOrEmpty(cartJson) ? new Cart() : JsonSerializer.Deserialize<Cart>(cartJson);

            if (cart.Items.Count == 0)
            {
                TempData["Error"] = "Your cart is empty. You cannot proceed to the payment page!";
                return RedirectToAction("Test", "Cart"); // Redirect to the cart page
            }

            return View(new UserInformation());
        }

        [HttpPost]
        public IActionResult Create(UserInformation model)
        {
            // If form validation fails, show the form again
            if (!ModelState.IsValid)
                return View(model);

            // Save user information to session (in JSON format)
            HttpContext.Session.SetString("UserInfo", JsonSerializer.Serialize(model));

            // Redirect to the payment page
            return RedirectToAction("Payment");
        }

        // Payment method selection page
        [HttpGet]
        public IActionResult Payment()
        {
            // Show the payment form with an empty PaymentInformation model
            return View(new PaymentInformation());
        }

        [HttpPost]
        public IActionResult Payment(PaymentInformation model)
        {
            // If payment method is not selected, show an error message
            if (model.PaymentMethod == null)
            {
                ModelState.AddModelError("", "You must select a payment method.");
                return View(model);
            }

            // Retrieve user information from session
            var userJson = HttpContext.Session.GetString("UserInfo");
            var userInfo = JsonSerializer.Deserialize<UserInformation>(userJson);

            // Get the cart information
            var cart = GetCart();

            // Create an order object
            var order = new Order
            {
                Id = new Random().Next(100000, 999999), // Random order number
                UserInformation = userInfo,
                Items = cart.Items.Select(x => new OrderItem
                {
                    ProductId = x.ProductId,
                    Name = x.Name,
                    Price = x.Price,
                    Quantity = x.Quantity
                }).ToList(),
                PaymentMethod = model.PaymentMethod
            };

            // Save the order to session
            HttpContext.Session.SetString(SessionOrderKey, JsonSerializer.Serialize(order));

            // Clear the cart (after creating the order)
            cart.Clear();
            HttpContext.Session.SetString(SessionCartKey, JsonSerializer.Serialize(cart));

            // Redirect to the order details page
            return RedirectToAction("Details");
        }

        // Order details page
        [HttpGet]
        public IActionResult Details()
        {
            // Retrieve the last order from session
            var json = HttpContext.Session.GetString(SessionOrderKey);
            // If no order exists, redirect to the homepage
            if (json == null)
                return RedirectToAction("Index", "Home");

            // Deserialize the JSON into an Order object and send it to the view
            var order = JsonSerializer.Deserialize<Order>(json);
            return View(order);
        }
    }
}