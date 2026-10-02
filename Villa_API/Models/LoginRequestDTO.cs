using System.ComponentModel.DataAnnotations;

namespace Villa_API.Models
{
    public class LoginRequestDTO
    {
        [Required]
        [EmailAddress]
        public required string Email { get; set; }

        [Required]

        public required string Password { get; set; }
    }
}
