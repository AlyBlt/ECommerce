using ECommerce.Application.DTOs.User;


namespace ECommerce.Application.DTOs.Auth
{
    public class LoginResponseDTO
    {
        public string Token { get; set; } = string.Empty;
        public UserDTO? User { get; set; }
    }
}
