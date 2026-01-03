using ECommerce.Application.DTOs.User;
using ECommerce.Domain.Entities;
using System.Collections.Generic;

namespace ECommerce.Application.Interfaces.Services
{
    public interface IUserService
    {
        Task<IEnumerable<UserDTO>> GetAllAsync();
        Task<UserDTO?> GetAsync(int id, bool includeOrders = false, bool includeProducts = false);
        Task<UserDTO?> GetByEmailAsync(string email);
        Task<UserDTO?> AuthenticateAsync(string email, string password);
        Task<UserDTO> RegisterAsync(UserDTO userDto);

        Task AddAsync(UserDTO userDto);
        Task UpdateAsync(UserDTO userDto);
        Task DeleteAsync(int id);

        Task ApproveSellerAsync(int id);
        Task RequestSellerStatusAsync(int userId);
        Task ToggleEnabledAsync(int userId);
        Task RejectSellerAsync(int id);





    }
}