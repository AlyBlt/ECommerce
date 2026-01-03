using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ECommerce.Domain.Entities
{
    public class OrderItemEntity
    {
        public int Id { get; set; }
        public int ProductId { get; set; }
        public int OrderId { get; set; }
        public decimal UnitPrice { get; set; }
        public byte Quantity { get; set; }
        public DateTime CreatedAt  { get; set; }
       

        // Navigation properties
        public OrderEntity? Order { get; set; }
        public ProductEntity? Product { get; set; } 
    }
}
