using System.ComponentModel.DataAnnotations;

namespace ECommerce.Admin.Mvc.Models.Category
{
    public class CategoryCreateEditViewModel
    {
        public int Id { get; set; }

        [Required]
        [StringLength(100, MinimumLength = 2)]
        public string Name { get; set; } = null!;

        [Required]
        [StringLength(6, MinimumLength = 3)]
        public string Color { get; set; } = null!;

        [Required]
        [StringLength(50, MinimumLength = 2)]
        public string IconCssClass { get; set; } = null!;
    }
}
