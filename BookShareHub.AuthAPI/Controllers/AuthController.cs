using BookShareHub.Application.Users.DTOs;
using BookShareHub.Application.Users.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BookShareHub.UsersAPI.Controllers
{
    /// <summary>
    /// Provides authentication endpoints.
    /// </summary>
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly AuthService _authService;

        /// <summary>
        /// Initializes a new instance of the <see cref="AuthController"/>.
        /// </summary>
        /// <param name="authService">The service responsible for authentication operations.</param>
        public AuthController(AuthService authService)
        {
            _authService = authService;
        }
  
        /// <summary>
        /// Authenticates a user and returns a JWT token.
        /// </summary>
        /// <param name="dto">
        /// The data transfer object containing the login credentials (email and password).
        /// </param>
        /// <returns>
        /// Returns a JWT token with status code 200 if authentication is successful,
        /// or 401 if credentials are invalid.
        /// </returns>
        [HttpPost("login")]
        [AllowAnonymous]
        [ProducesResponseType(typeof(object), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<IActionResult> Login([FromBody] LoginUserDto dto)
        {
            var token = await _authService.LoginAsync(dto);

            if (token == null) return Unauthorized();

            return Ok(new { Token = token });
        }

        /// <summary>
        /// Logs out the current user.
        /// </summary>
        /// <returns>
        /// Returns a confirmation message with status code 200.
        /// Note: With JWT authentication, logout is handled client-side by discarding the token.
        /// </returns>
        [HttpPost("logout")]
        [Authorize]
        [ProducesResponseType(typeof(object), StatusCodes.Status200OK)]
        public IActionResult Logout()
        {
            return Ok(new { Message = "Logged out successfully. Discard your JWT token." });
        }
    }
}
