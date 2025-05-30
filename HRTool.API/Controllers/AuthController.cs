using HRTool.API.Services;
using HRTool.Domain.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System.ComponentModel.DataAnnotations;
using System.Threading.Tasks;

namespace HRTool.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly IAuthService _authService;
        private readonly ILogger<AuthController> _logger;

        public AuthController(IAuthService authService, ILogger<AuthController> logger)
        {
            _authService = authService;
            _logger = logger;
        }

        /// <summary>
        /// Authenticates a user and returns a JWT token if successful.
        /// </summary>
        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginRequest request)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);
            var token = await _authService.AuthenticateAsync(request.Email, request.Password);
            if (token == null)
            {
                _logger.LogWarning("Failed login attempt for email: {Email}", request.Email);
                return Unauthorized();
            }
            _logger.LogInformation("User logged in: {Email}", request.Email);
            return Ok(new { token });
        }
    }

    /// <summary>
    /// DTO for login requests.
    /// </summary>
    public class LoginRequest
    {
        [Required]
        [EmailAddress]
        public string Email { get; set; } = string.Empty;

        [Required]
        [StringLength(100, MinimumLength = 6)]
        public string Password { get; set; } = string.Empty;
    }
}
