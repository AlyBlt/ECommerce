using System.ComponentModel.DataAnnotations;

namespace ECommerceWeb.MVC.Models.OrderviewModels
{
    public class Order
    {
        public int Id { get; set; }
        public DateTime CreatedDate { get; set; } = DateTime.Now;
        public List<OrderItem> Items { get; set; } = new List<OrderItem>();
        public decimal TotalPrice => Items.Sum(x => x.TotalPrice);
        public string PaymentMethod { get; set; }
        public UserInformation UserInformation { get; set; }
    }
}