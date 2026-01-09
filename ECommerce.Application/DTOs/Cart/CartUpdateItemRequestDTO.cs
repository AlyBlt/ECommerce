

namespace ECommerce.Application.DTOs.Cart
{
    public class CartUpdateItemRequestDTO
    {
        public int UserId { get; set; }
        public int ProductId { get; set; }
        public int Quantity { get; set; }
    }
}
