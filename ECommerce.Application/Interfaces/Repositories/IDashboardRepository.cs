using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ECommerce.Application.Interfaces.Repositories
{
    public interface IDashboardRepository
    {
        Task<int> GetTotalUsersAsync();
        Task<int> GetTotalProductsAsync();
        Task<int> GetTotalCategoriesAsync();
        Task<int> GetPendingCommentsAsync();
        Task<int> GetPendingSellersAsync();
    }
}
