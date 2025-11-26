using ECommerce.Data.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ECommerce.Application.ViewModels
{
    public class ProductListingViewModel
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public int CategoryId { get; set; }
        public string Category { get; set; }
        public string ImageUrl { get; set; }
        public decimal Price { get; set; }

        public double Rating { get; set; }
        public int ReviewCount { get; set; }

        // İndirim ve satış durumu
        public decimal? OldPrice { get; set; }
        public bool IsOnSale => OldPrice.HasValue && OldPrice > Price;  // OldPrice > Price ise indirimli
                                                                        // Yeni Gelen Ürünler için CreatedAt özelliği
        public DateTime CreatedAt { get; set; }

        // Sepette mi?
        public bool InCart { get; set; } = false;

        public List<CommentViewModel> Comments { get; set; } = new List<CommentViewModel>();
    }
}
