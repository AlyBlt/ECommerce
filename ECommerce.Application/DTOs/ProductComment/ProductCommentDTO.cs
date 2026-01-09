

namespace ECommerce.Application.DTOs.ProductComment
{
    public class ProductCommentDTO
    {
        public int Id { get; set; }
        public int ProductId { get; set; }
        public string? ProductName { get; set; }
        public int UserId { get; set; }
        public string? UserName { get; set; }
        public string Text { get; set; } = null!;
        public byte StarCount { get; set; }
        public bool IsConfirmed { get; set; }
        public bool IsRejected { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}
