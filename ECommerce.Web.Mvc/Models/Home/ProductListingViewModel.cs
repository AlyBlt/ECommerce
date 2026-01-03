using ECommerce.Web.Mvc.Models.Category;
using ECommerce.Web.Mvc.Models.Comment;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ECommerce.Web.Mvc.Models.Home
{
    public class ProductListingViewModel
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public int CategoryId { get; set; }
        public string Category { get; set; } //category name olarak alabiliriz
        public string ImageUrl { get; set; }
        public decimal Price { get; set; }

        public double Rating { get; set; }
        public int ReviewCount { get; set; }
        public bool Enabled { get; set; } = true;

        // İndirim ve satış durumu
        public decimal? OldPrice { get; set; }
        public bool IsOnSale => OldPrice.HasValue && OldPrice > Price;  // OldPrice > Price ise indirimli
                                                                        // Yeni Gelen Ürünler için CreatedAt özelliği
        public DateTime CreatedAt { get; set; }

        // Sepette mi?
        public bool InCart { get; set; } = false;

        // Sayfalama ve Filtreleme Özellikleri
        public int CurrentPage { get; set; }  // Şu anki sayfa
        public int TotalPages { get; set; }    // Toplam sayfa sayısı
        public string Filter { get; set; }     // Uygulanan filtre (örneğin, "new-arrivals")

        public string Query { get; set; }
        public List<ProductListingViewModel> Products { get; set; }  // Ürünler listesi

        public List<CommentViewModel> Comments { get; set; } = new List<CommentViewModel>();
        public List<CategoryViewModel> Categories { get; set; } = new();

    }


}
