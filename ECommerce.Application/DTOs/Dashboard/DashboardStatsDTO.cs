

namespace ECommerce.Application.DTOs.Dashboard
{
    public class DashboardStatsDTO
    {
        public int TotalUsers { get; set; }
        public int TotalProducts { get; set; }
        public int TotalCategories { get; set; }
        public int PendingComments { get; set; }
        public int PendingSellers { get; set; }
        public decimal DailySales { get; set; }
    }
}
