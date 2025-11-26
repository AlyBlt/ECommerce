using ECommerce.Data.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ECommerce.Data.Entities
{
    public class ProductEntity
    {
        public int Id { get; set; }
        public int CategoryId { get; set; }
        public int SellerId { get; set; }
        public string Name { get; set; } = null!;
        public decimal Price { get; set; }
        public decimal? OldPrice { get; set; }
        public string? Details { get; set; }
        public byte StockAmount { get; set; }
        public bool Enabled { get; set; } = true;
        public DateTime CreatedAt { get; set; }
        public string ImageUrl { get; set; }

        // Yeni eklenen IsFeatured alanı
        public bool IsFeatured { get; set; } = false;  // Varsayılan olarak false, öne çıkan olmayan ürünler

        // Navigation
        public CategoryEntity? Category { get; set; }
        public UserEntity? Seller { get; set; }
        public ICollection<ProductCommentEntity>? Comments { get; set; }
        public ICollection<ProductImageEntity>? Images { get; set; }
        // Rating hesaplama
        public decimal Rating
        {
            get
            {
                // Eğer yorum yoksa 0 döner
                if (Comments == null || !Comments.Any()) return 0;

                // Yorumlardaki puanların ortalamasını decimal tipine dönüştürerek döner
                return (decimal)Comments.Average(c => (decimal)c.StarCount);
            }
        }

    }
}
