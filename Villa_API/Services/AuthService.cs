using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Villa_API.Data;
using Villa_API.Models;

namespace Villa_API.Services
{
    public class AuthService : IAuthService
    {
        private readonly ApplicationDbContext _db;
        private readonly IMapper _mapper;
        private readonly IConfiguration _configuration;
        public AuthService(ApplicationDbContext db, IConfiguration configuration, IMapper mapper)
        {
            _db = db;
            _mapper = mapper;
            _configuration = configuration;
        }

        public async Task<bool> IsEmailExistsAsync(string email)
        {
            return await _db.Users.AnyAsync(u => u.Email.ToLower() == email.ToLower());
        }

        public Task<LoginRequestDTO> LoginAsync(LoginRequestDTO loginRequestDTO)
        {
            throw new NotImplementedException();
        }

        public async Task<UserDTO?> RegisterAsync(RegistrationRequestDTO registrationRequestDTO)
        {

            try
            {
                if (await IsEmailExistsAsync(registrationRequestDTO.Email))
                {
                    throw new Exception($"Email {registrationRequestDTO.Email} is already registered");
                }

                User user = new User
                {
                    Email = registrationRequestDTO.Email,
                    Name = registrationRequestDTO.Name,
                    Password = registrationRequestDTO.Password,
                    Role = string.IsNullOrEmpty(registrationRequestDTO.Role) ? "Customer" : registrationRequestDTO.Role,
                    CreatedDate = DateTime.Now,

                };

                await _db.Users.AddAsync(user);
                await _db.SaveChangesAsync();

                return _mapper.Map<UserDTO>(user);
            }
            catch (Exception ex)
            {
                throw new Exception($"An error occurred while registering the user: {ex.Message}");
            }
          
        }
    }
}
