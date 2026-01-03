using ECommerce.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ECommerce.Application.Interfaces.Repositories
{
    public interface IUserRepository : IBaseRepository<UserEntity>
    {
        Task<List<UserEntity>> GetAllWithRolesAsync();
        Task<UserEntity?> GetByIdWithIncludesAsync(int id, bool includeOrders = false, bool includeProducts = false);
        Task<UserEntity?> GetByEmailAsync(string email);
    }
   
}
