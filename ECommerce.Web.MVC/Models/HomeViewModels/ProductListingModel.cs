using ECommerceWeb.MVC.Models.ProductViewModels;

namespace ECommerceWeb.MVC.Models.HomeViewModels
{
    public class ProductListingModel
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Category { get; set; }
        public string ImageUrl { get; set; }
        public decimal Price { get; set; }
        public decimal? OldPrice { get; set; } // İndirim varsa
        public bool IsOnSale => OldPrice.HasValue;

        public List<ProductComment> Comments { get; set; } = new List<ProductComment>();
    }
}
