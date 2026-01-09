using ECommerce.Application.DTOs.Cart;
using ECommerce.Application.DTOs.Order;
using ECommerce.Application.Interfaces.Services;

namespace ECommerce.Admin.Mvc.Services
{
    public class OrderApiService : IOrderService
    {
        private readonly HttpClient _httpClient;

        public OrderApiService(IHttpClientFactory httpClientFactory)
        {
            // JWT Token taşıyan yapılandırılmış client
            _httpClient = httpClientFactory.CreateClient("DataApi");
        }

        // Sipariş Oluşturma
        public async Task<OrderDTO> CreateOrderAsync(int userId, string address, string paymentMethod, List<CartItemDTO> cartItems, string deliveryFullName, string deliveryPhone)
        {
            // API'ye gönderilecek veriyi bir nesneye paketleyelim
            var orderRequest = new
            {
                UserId = userId,
                Address = address,
                PaymentMethod = paymentMethod,
                CartItems = cartItems,
                DeliveryFullName = deliveryFullName,
                DeliveryPhone = deliveryPhone
            };

            var response = await _httpClient.PostAsJsonAsync("api/order/create", orderRequest);
            response.EnsureSuccessStatusCode();

            return await response.Content.ReadFromJsonAsync<OrderDTO>()
                   ?? throw new Exception("Sipariş oluşturuldu fakat veri alınamadı.");
        }

        // Kullanıcının Siparişlerini Listeleme
        public async Task<IEnumerable<OrderDTO>> GetOrdersByUserAsync(int userId)
        {
            return await _httpClient.GetFromJsonAsync<IEnumerable<OrderDTO>>($"api/order/user/{userId}")
                   ?? new List<OrderDTO>();
        }

        // Sipariş Detayı Getirme
        public async Task<OrderDTO?> GetOrderAsync(int orderId)
        {
            return await _httpClient.GetFromJsonAsync<OrderDTO>($"api/order/{orderId}");
        }
    }
}
