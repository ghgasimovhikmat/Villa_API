using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Villa_API.DTO;
using Villa_API.Models;
using Villa_API.Services;

namespace Villa_API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly IAuthService _authService;
        public AuthController(IAuthService authService)
        {
            _authService = authService;
        }
        [HttpGet]
        [ProducesResponseType(typeof(ApiResponse<IEnumerable<VillaDTO>>),StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<ApiResponse<UserDTO>>>Register(RegistrationRequestDTO registrationRequestDTO)
        {
            try
            {
                               if (registrationRequestDTO == null)
                {
                    return BadRequest(ApiResponse<object>.BadRequest("Villa data is required"));
                }
                //auth service
                if (await _authService.IsEmailExistingAsync(registrationRequestDTO.Email))
                {
                    return Conflict(ApiResponse<object>.Conflict($"Email {registrationRequestDTO.Email} is already registered"));
                }

                var user = await _authService.RegisterAsync(registrationRequestDTO);
                if (user == null)
                {
                    return BadRequest(ApiResponse<object>.BadRequest("Registration Failed"));
                }
                //auth service
                var response = ApiResponse<UserDTO>.CreatedAt(user, "User registered successfully");
                return CreatedAtAction(nameof(Register), response);
            }

            catch (Exception ex)
            {
                var response = ApiResponse<object>.Error(StatusCodes.Status500InternalServerError, $"An error occurred while creating the villa: {ex.Message}");
                return StatusCode(500, response);
            }
        }
    }
}
