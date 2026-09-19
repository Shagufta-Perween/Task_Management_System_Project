using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using TaskManagement.API.DTOs.Auth;
using TaskManagement.API.Services;

namespace TaskManagement.API.Controllers
{
    /// <summary>
    /// User authentication and registration management (Public Endpoints)
    /// </summary>
    [ApiController]
    [Route("api/[controller]")]
    [AllowAnonymous]
    [Produces("application/json")]
    public class AuthController : ControllerBase
    {
        private readonly IAuthService _authService;

        public AuthController(IAuthService authService)
        {
            _authService = authService;
        }

        /// <summary>
        /// Register a new user account
        /// </summary>
        /// <remarks>
        /// Public endpoint. Creates a new user profile with default 'User' role and returns a JWT token for immediate access. No existing token required.
        /// </remarks>
        /// <param name="dto">User registration details including full name, email, and password.</param>
        /// <returns>Authentication response containing JWT token, user details, and expiration.</returns>
        /// <response code="200">User created successfully and token returned.</response>
        /// <response code="400">Invalid payload, missing fields, or email already registered.</response>
        [HttpPost("register")]
        [ProducesResponseType(typeof(AuthResponseDto), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ValidationProblemDetails), StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<AuthResponseDto>> Register([FromBody] RegisterDto dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var result = await _authService.RegisterAsync(dto);
            return Ok(result);
        }

        /// <summary>
        /// Authenticate user and issue JWT token
        /// </summary>
        /// <remarks>
        /// Public endpoint. Validates email and password credentials and returns bearer token on success.
        /// </remarks>
        /// <param name="dto">Login credentials (email and password).</param>
        /// <returns>Authentication response with JWT bearer token and user role info.</returns>
        /// <response code="200">Credentials verified and token issued.</response>
        /// <response code="400">Invalid email/password format or authentication credentials.</response>
        [HttpPost("login")]
        [ProducesResponseType(typeof(AuthResponseDto), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ValidationProblemDetails), StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<AuthResponseDto>> Login([FromBody] LoginDto dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var result = await _authService.LoginAsync(dto);
            return Ok(result);
        }
    }
}
