namespace ECommerce.Admin.Mvc.Models.Product
{
    public class ProductListViewModel
    {
        public int Id { get; set; }
        public string Name { get; set; } = null!;
        public string CategoryName { get; set; } = null!;
        public decimal Price { get; set; }
        public int SellerId { get; set; }
        public string SellerName { get; set; } 
        public bool Enabled { get; set; }

        // Yeni eklenen alan: Ürünün ana resminin dosya adını tutacak
        public string? MainImageUrl { get; set; }

    }
}
