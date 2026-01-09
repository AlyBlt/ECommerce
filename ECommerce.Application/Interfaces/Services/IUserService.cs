using ECommerce.Application.DTOs.Auth;
using ECommerce.Application.DTOs.User;


namespace ECommerce.Application.Interfaces.Services
{
    public interface IUserService
    {
        Task<IEnumerable<UserDTO>> GetAllAsync();
        Task<UserDTO?> GetAsync(int id, bool includeOrders = false, bool includeProducts = false);
        Task<UserDTO?> GetByEmailAsync(string email);
        Task<LoginResponseDTO?> AuthenticateAsync(string email, string password);
        Task<UserDTO> RegisterAsync(UserDTO userDto);

        Task AddAsync(UserDTO userDto);
        Task UpdateAsync(UserDTO userDto);
        Task DeleteAsync(int id);

        Task ApproveSellerAsync(int id);
        // Admin tarafı veya sistem tarafı için (ID zorunlu)
        Task RequestSellerStatusAsync(int userId);

        // Giriş yapmış kullanıcının kendisi için (ID dışarıdan gelmez, Token'dan okunur)
        Task RequestSellerStatusAsync();
        Task ToggleEnabledAsync(int userId);
        Task RejectSellerAsync(int id);

        //yeni
        Task<bool> InitiatePasswordResetAsync(string email);
        Task<bool> VerifyResetTokenAsync(string token);
        Task<bool> ResetPasswordAsync(string token, string newPassword);

        Task<UserDTO?> GetCurrentUserAsync(int userId, bool includeOrders = false, bool includeProducts = false);
        Task UpdateCurrentUserAsync(UserDTO dto);



    }
}