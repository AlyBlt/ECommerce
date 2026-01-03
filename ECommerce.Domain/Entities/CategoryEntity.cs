using System;
using System.ComponentModel.DataAnnotations;

namespace ECommerce.Domain.Entities
{
        public class CategoryEntity
        {
        public int Id { get; set; }
        [MinLength(2)]
        public string Name { get; set; } = null!;
        [MinLength(3)]
        public string Color { get; set; } = null!;
        [MinLength(2)]
        public string IconCssClass { get; set; } = null!;
        public DateTime CreatedAt { get; set; }

        // Navigation
        public ICollection<ProductEntity>? Products { get; set; }
        }
    
    }
