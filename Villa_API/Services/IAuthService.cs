using Villa_API.Models;

namespace Villa_API.Services
{
    public interface IAuthService
    {
        Task<UserDTO> RegisterAsync(RegistrationRequestDTO registrationRequestDTO);
        Task<LoginRequestDTO> LoginAsync(LoginRequestDTO loginRequestDTO);

        Task<bool> IsEmailExistsAsync(string email);

    }
}
