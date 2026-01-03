using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ECommerce.Web.Mvc.Models.Favorite

{
    public class FavoriteItemViewModel
    {
        public int ProductId { get; set; }
        public string Name { get; set; }
        public decimal Price { get; set; }
        public string ImageUrl { get; set; } // Resim URL'sini burada tutuyoruz
    }
}
