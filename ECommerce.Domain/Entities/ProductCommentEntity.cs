using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ECommerce.Domain.Entities
{
    public class ProductCommentEntity
    {
        public int Id { get; set; }
        public int ProductId { get; set; }
        public int UserId { get; set; }
        public string Text { get; set; } = null!; 
        public byte StarCount { get; set; }
        public bool IsConfirmed { get; set; } = false;
        public bool IsRejected { get; set; } = false;
        public DateTime CreatedAt { get; set; }

        public ProductEntity Product { get; set; } = null!;
        public UserEntity? User { get; set; }       

    }
}
