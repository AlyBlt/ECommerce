using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ECommerce.Domain.Entities
{
    public class ProductImageEntity
    {
        public int Id { get; set; }
        public int ProductId { get; set; }
        public string Url { get; set; } = null!;
        public bool IsMain { get; set; } = false;  // Ana görsel mi?
        public DateTime CreatedAt { get; set; }
        public ProductEntity? Product { get; set; }
    }
}
