using ECommerce.Application.DTOs.Role;
using ECommerce.Application.Interfaces.Repositories;
using ECommerce.Application.Interfaces.Services;
using ECommerce.Domain.Entities;


namespace ECommerce.Application.Services
{
    public class RoleService : IRoleService
    {
        private readonly IRoleRepository _roleRepository;

        public RoleService(IRoleRepository roleRepository)
        {
            _roleRepository = roleRepository;
        }


        public async Task<IEnumerable<RoleDTO>> GetAllAsync()
        {
            var roles = await _roleRepository.GetAllAsync();
            return roles.Select(r => new RoleDTO
            {
                Id = r.Id,
                Name = r.Name,
                CreatedAt = r.CreatedAt,
                // Eğer Repository Include(Users) yapıyorsa sayısı da gelir
                UserCount = r.Users?.Count ?? 0
            });
        }

        public async Task<RoleDTO?> GetAsync(int id)
        {
            var role = await _roleRepository.GetByIdAsync(id);
            if (role == null) return null;

            return new RoleDTO
            {
                Id = role.Id,
                Name = role.Name,
                CreatedAt = role.CreatedAt
            };
        }

        public async Task AddAsync(RoleDTO roleDto)
        {
            var role = new RoleEntity
            {
                Name = roleDto.Name,
                CreatedAt = DateTime.Now
            };
            await _roleRepository.AddAsync(role);
            await _roleRepository.SaveAsync();
        }

        public async Task UpdateAsync(RoleDTO roleDto)
        {
            var role = await _roleRepository.GetByIdAsync(roleDto.Id);
            if (role != null)
            {
                role.Name = roleDto.Name;
                await _roleRepository.UpdateAsync(role);
                await _roleRepository.SaveAsync();
            }
        }

        public async Task DeleteAsync(int id)
        {
            var role = await _roleRepository.GetByIdAsync(id);
            if (role != null)
            {
                await _roleRepository.DeleteAsync(role);
                await _roleRepository.SaveAsync();
            }
        }
    }
}