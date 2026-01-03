using ECommerce.Application.DTOs.ProductComment;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ECommerce.Application.DTOs.Product
{
    public class ProductDTO
    {
        public int Id { get; set; }
        public int CategoryId { get; set; }
        public string CategoryName { get; set; } = null!;
        public int SellerId { get; set; }
        public string SellerName { get; set; } = null!;
        public string Name { get; set; } = null!;
        public decimal Price { get; set; }
        public decimal? OldPrice { get; set; }
        public string? Details { get; set; }
        public byte StockAmount { get; set; }
        public bool Enabled { get; set; }
        public bool IsFeatured { get; set; }
        public DateTime CreatedAt { get; set; }

        // Görsel yönetimi
        public string? MainImageUrl { get; set; }
        public List<string> AllImageUrls { get; set; } = new();

        // Rating ve Hesaplamalar
        public byte Rating { get; set; }
        public int ReviewCount { get; set; }

        // UI Mantığı (IsOnSale ViewModel'de de vardı, DTO'da olması işi kolaylaştırır)
        public bool IsOnSale => OldPrice.HasValue && OldPrice > Price;
        public List<ProductCommentDTO> Comments { get; set; } = new List<ProductCommentDTO>();
    }
}
