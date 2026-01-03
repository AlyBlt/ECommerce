using ECommerce.Application.Interfaces.Repositories;
using ECommerce.Data.DbContexts;
using ECommerce.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

namespace ECommerce.Data.Repositories
{
    internal class CategoryRepository : BaseRepository<CategoryEntity>, ICategoryRepository
    {
        public CategoryRepository(ECommerceDbContext context) : base(context)
        {
        }

        public async Task<bool> HasProductsAsync(int categoryId)
        {
            return await Context.Products
                .AnyAsync(p => p.CategoryId == categoryId);
        }
    }
}
