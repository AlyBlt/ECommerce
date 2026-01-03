using ECommerce.Application.DTOs.Role;
using ECommerce.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ECommerce.Application.Interfaces.Services
{
    public interface IRoleService
    {
        Task<IEnumerable<RoleDTO>> GetAllAsync();
        Task<RoleDTO?> GetAsync(int id);
        Task AddAsync(RoleDTO roleDto);
        Task UpdateAsync(RoleDTO roleDto);
        Task DeleteAsync(int id);
    }
}
