namespace Villa_API.Models
{
    public class LoginResponseDTO
    {
        public string? Token { get; set; }

        public UserDTO? userDTO { get; set; }
    }
}
