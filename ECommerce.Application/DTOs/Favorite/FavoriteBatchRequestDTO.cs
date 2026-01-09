

namespace ECommerce.Application.DTOs.Favorite
{
    public class FavoriteBatchRequestDTO
    {
        public int UserId { get; set; }
        public List<int> ProductIds { get; set; }
    }
}
