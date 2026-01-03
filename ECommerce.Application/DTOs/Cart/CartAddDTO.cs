using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ECommerce.Application.DTOs.Cart
{
    public class CartAddDTO
    {
        public int ProductId { get; set; }
        public byte Quantity { get; set; }
    }
}
