using System.ComponentModel.DataAnnotations;

namespace ECommerceWeb.MVC.Models
{
    public class ProductViewModel
    {
        /// <summary>
        /// Ürün Id
        /// </summary>
        public int ProductId { get; set; }
        /// <summary>
        /// Ürün Adı
        /// </summary>
        public string ProductName { get; set; }
        /// <summary>
        /// Fiyat
        /// </summary>
        public decimal ProductPrice { get; set; }
        /// <summary>
        /// Açıklama
        /// </summary>
        public string ProductDescription { get; set; }

        /// <summary>
        /// Ürün Yorumu
        /// </summary>
        public string ProductComment { get; set; }

        /// <summary>
        /// Ürün beğeni yıldızı
        /// Null olabilir
        /// İllk ürün oluştuğunda zaten yıldız veremeyiz. ancak satın aldıktan sonra
        /// </summary>
        [Length(1, 5)]
        public short? ProductStar { get; set; }

        /// <summary>
        /// Kategori
        /// </summary>
        public CategoryViewModel Category { get; set; }


    }
}
