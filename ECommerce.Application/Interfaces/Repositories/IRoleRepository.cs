using ECommerce.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ECommerce.Application.Interfaces.Repositories
{
    public interface IRoleRepository : IBaseRepository<RoleEntity>
    {
        Task<RoleEntity?> GetByNameAsync(string roleName);
    }
}
