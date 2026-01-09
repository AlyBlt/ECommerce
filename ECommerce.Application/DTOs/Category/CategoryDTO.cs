

namespace ECommerce.Application.DTOs.Category
{
    public class CategoryDTO
    {
        public int Id { get; set; }
        public string Name { get; set; } = null!;
        public string Color { get; set; } = null!;
        public string IconCssClass { get; set; } = null!;

        // Opsiyonel: Kategoriye ait kaç ürün olduğunu göstermek istersen
        public int ProductCount { get; set; }
        // Yeni eklenen alan
        public string? ImageUrl { get; set; }
    }
}
