using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ECommerce.Application.ViewModels
{
    public class ProductDetailViewModel
    {
        // Temel ürün bilgileri
        public int Id { get; set; }
        public string Name { get; set; }
        public string Category { get; set; }
        public string ImageUrl { get; set; }
        public decimal Price { get; set; }
        public decimal? OldPrice { get; set; }
        public bool IsOnSale => OldPrice.HasValue;

        // Detaylar
        public string Description { get; set; }
        public string Availability { get; set; }

        // Review ve rating
        public double Rating { get; set; }
        public int ReviewCount { get; set; }
        public List<CommentViewModel> Comments { get; set; } = new List<CommentViewModel>();

        // Related products
        public List<ProductListingViewModel> RelatedProducts { get; set; } = new List<ProductListingViewModel>();

    }
}
