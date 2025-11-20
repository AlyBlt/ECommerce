namespace ECommerceWeb.MVC.Models.HomeViewModels
{
    public class ProductDetailViewModel
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string MainImageUrl { get; set; }
        public string Category { get; set; }
        public List<string> GalleryImages { get; set; }
        public decimal Price { get; set; }
        public decimal? OldPrice { get; set; }
        public double Rating { get; set; }
        public int ReviewCount { get; set; }
        public string Description { get; set; }
        public string Information { get; set; }
        public string Availability { get; set; }
        public string Shipping { get; set; }
        public double Weight { get; set; }
        public List<ProductListingModel> RelatedProducts { get; set; }
    }
}
