using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ECommerce.Data.Entities
{
    public class DashboardStatsEntity
    {
        public int TotalUsers { get; set; }
        public int TotalProducts { get; set; }
        public int TotalCategories { get; set; }
        public int PendingComments { get; set; }
        public int PendingSellers { get; set; }
        public decimal DailySales { get; set; } // Günlük satış varsa
    }
}
