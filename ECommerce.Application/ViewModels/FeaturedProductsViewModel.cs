using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ECommerce.Application.ViewModels
{
    public class FeaturedProductsViewModel
    {
        public List<ProductListingViewModel> PopularProducts { get; set; }
        public List<ProductListingViewModel> DiscountedProducts { get; set; }
        public List<ProductListingViewModel> NewArrivalProducts { get; set; }
    }
}
