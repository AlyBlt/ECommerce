namespace ECommerceWeb.MVC.Models.ProductViewModels
{
    public class ProductComment
    {
        public string UserName { get; set; }
        public int Rating { get; set; } // 1-5
        public DateTime CreatedAt { get; set; }
        public string CommentText { get; set; }
    }
}
