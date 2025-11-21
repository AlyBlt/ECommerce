namespace ECommerceWeb.MVC.Models.FavoritesViewModels
{
    public class FavoriteItem
    {
        public int ProductId { get; set; }
        public string Name { get; set; }
        public decimal Price { get; set; }
        public string ImageUrl { get; set; } // Resim URL'sini burada tutuyoruz
    }
}
