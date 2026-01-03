using ECommerce.Web.Mvc.Models.Category;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ECommerce.Web.Mvc.Models.Cart
{
    public  class CartViewModel
    {
        public List<CartItemViewModel> Items { get; set; } = new();
        public decimal TotalPrice => Items.Sum(i => i.TotalPrice);
    }
}
