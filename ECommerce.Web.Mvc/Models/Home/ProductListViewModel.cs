using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ECommerce.Web.Mvc.Models.Home
{
    public class ProductListViewModel
    {
        public int Id { get; set; }
        public string Name { get; set; } = "";
        public decimal Price { get; set; }
        public bool IsActive { get; set; }

        public string ImageUrl { get; set; }

        public byte? Rating { get; set; }
        public int? ReviewCount { get; set; }
        public decimal? OldPrice { get; set; }
        public bool IsOnSale => OldPrice.HasValue;

    }
}
