using System.ComponentModel.DataAnnotations;

namespace Villa_API.Models
{
    public class RegistrationRequestDTO
    {
        [Required]
        [EmailAddress]
        public required string Email { get; set; }

        [Required]
        [MaxLength(100)]

        public required string Name { get; set; }
        [Required]

        public required string Password { get; set; }

        [MaxLength(100)]
        public string Role { get; set; } = "Customer";

    }
}
