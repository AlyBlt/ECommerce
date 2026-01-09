using System.ComponentModel.DataAnnotations;

namespace ECommerce.Web.Mvc.Models.Product
{
    public class ProductCreateViewModel
    {
        [Required, MinLength(2), MaxLength(100)]
        public string Name { get; set; }

        [Required]
        [Range(0.1, 999999)]
        public decimal Price { get; set; }

        [Range(0.1, 999999)]
        public decimal? OldPrice { get; set; }

        [Required]
        public int CategoryId { get; set; }

        [Required]
        [Range(0, 255)]
        public byte StockAmount { get; set; }

        [MaxLength(1000)]
        public string Details { get; set; }
        public string ImageUrl { get; set; }
        public IFormFile? ImageFile { get; set; }

    }
}
