namespace ECommerceWeb.MVC.Models
{
    // Sepet sınıfı: Kullanıcının sepete eklediği ürünleri tutar
    public class Cart
    {
        // Sepetteki ürünleri tutan liste
        // Başlangıçta boş bir liste oluşturuluyor, null hatası önleniyor
        public List<CartItem> Items { get; set; } = new List<CartItem>();

        // Sepetin toplam fiyatını hesaplayan özellik
        // Items listesindeki her bir ürünün TotalPrice'ını toplar
        public decimal TotalPrice => Items.Sum(x => x.TotalPrice);

        // Sepete ürün ekleme metodu
        public void AddItem(CartItem item)
        {
            // Sepette aynı üründen var mı diye kontrol ediyoruz
            var existing = Items.FirstOrDefault(x => x.ProductId == item.ProductId);
            // Ürün zaten varsa, sadece miktarını artırıyoruz
            if (existing != null)
                existing.Quantity += item.Quantity;
            else
                // Ürün yoksa sepete yeni ekliyoruz
                Items.Add(item);
        }
        public void Clear() => Items.Clear();

    }
}
