namespace ECommerce.Admin.Mvc.Models.Product
{
    public class ProductDeleteViewModel
    {
        public int Id { get; set; }
        public string Name { get; set; } = null!;
        public string CategoryName { get; set; } = null!;
        public decimal Price { get; set; }
    }
}
