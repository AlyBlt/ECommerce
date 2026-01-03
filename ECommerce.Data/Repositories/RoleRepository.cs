using ECommerce.Application.Interfaces.Repositories;
using ECommerce.Data.DbContexts;
using ECommerce.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ECommerce.Data.Repositories
{
    internal class RoleRepository: BaseRepository<RoleEntity>, IRoleRepository
    {
        public RoleRepository(ECommerceDbContext context) : base(context) { }

        // Rolü isim ile getir
        public async Task<RoleEntity?> GetByNameAsync(string roleName)
        {
            return await GetQueryable()
                .FirstOrDefaultAsync(r => r.Name == roleName);
        }
    }
}
