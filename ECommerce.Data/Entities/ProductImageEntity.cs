using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ECommerce.Data.Entities
{
    public class ProductImageEntity
    {
        public Guid Id { get; set; }
        public int ProductId { get; set; }
        public string Url { get; set; }
        public DateTime CreatedAt { get; set; }
        public ProductEntity? Product { get; set; }
    }
}
