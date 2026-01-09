namespace ECommerce.Admin.Mvc.Models.Category
{
    public class CategoryListViewModel
    {
        public int Id { get; set; }
        public string Name { get; set; } = "";
        public string Color { get; set; } = "";
        public string IconCssClass { get; set; } = "";

        // Yeni eklenen alan
        public string? ImageUrl { get; set; }
        public IFormFile? ImageFile { get; set; }
    }
}
