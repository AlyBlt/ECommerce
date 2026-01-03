using ECommerce.Application.Interfaces.Repositories;
using ECommerce.Data.DbContexts;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ECommerce.Data.Repositories
{
    internal class DashboardRepository : IDashboardRepository
    {
        private readonly ECommerceDbContext _context;

        public DashboardRepository(ECommerceDbContext context)
        {
            _context = context;
        }

        public Task<int> GetTotalUsersAsync()
            => _context.Users.CountAsync();

        public Task<int> GetTotalProductsAsync()
            => _context.Products.CountAsync();

        public Task<int> GetTotalCategoriesAsync()
            => _context.Categories.CountAsync();

        public Task<int> GetPendingCommentsAsync()
            => _context.ProductComments.CountAsync(pc => !pc.IsConfirmed);

        public Task<int> GetPendingSellersAsync()
            => _context.Users.CountAsync(u => !u.IsSellerApproved);
    }
}
