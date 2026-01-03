using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ECommerce.Web.Mvc.Models.Comment
{
    public class CommentViewModel
    {

        public int Id { get; set; }
        public string UserName { get; set; } = null!;
        public string ProductName { get; set; } = null!;
        public decimal? OldPrice { get; set; }

        [Required(ErrorMessage = "Comment text is required."), StringLength(500)]
        public string Text { get; set; } = null!;
        public byte Rating { get; set; }
        public bool IsApproved { get; set; } = true;


    }
}
