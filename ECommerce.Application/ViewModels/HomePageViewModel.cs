using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ECommerce.Application.ViewModels
{
    public class HomePageViewModel
    {
        public FeaturedProductsViewModel Featured { get; set; }

        public List<ProductListingViewModel> Products { get; set; }
            = new List<ProductListingViewModel>();

        public List<CategoryViewModel> Categories { get; set; }
            = new List<CategoryViewModel>();

        public List<ProductListingViewModel> FeaturedProducts { get; set; }
    }
}
