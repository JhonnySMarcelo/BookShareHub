using BookShareHub.Application.Users.DTOs;
using BookShareHub.Application.Users.Services;
using BookShareHub.Domain.Users.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace BookShareHub.UsersAPI.Controllers
{
    /// <summary>
    /// Provides endpoints to manage users in the system.
    /// </summary>
    [ApiController]
    [Route("api/[controller]")]
    public class UsersController : ControllerBase
    {
        private readonly UserService _userService;

        /// <summary>
        /// Initializes a new instance of the <see cref="UsersController"/>.
        /// </summary>
        /// <param name="userService">The service responsible for user operations.</param>
        public UsersController(UserService userService)
        {
            _userService = userService;
        }

        /// <summary>
        /// Registers a new user in the system.
        /// </summary>
        /// <param name="request">
        /// The data transfer object containing the user details such as username, email, and password.
        /// </param>
        /// <returns>
        /// Returns the created <see cref="User"/> with status code 201 if successful,
        /// or a <see cref="ValidationProblemDetails"/> with status code 400 if validation fails.
        /// </returns>
        [HttpPost("register")]
        [AllowAnonymous]
        [ProducesResponseType(typeof(User), StatusCodes.Status201Created)]
        [ProducesResponseType(typeof(ValidationProblemDetails), StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<User>> Register([FromBody] RegisterUserDto request)
        {
            var user = await _userService.CreateAsync(request);
            return Created($"api/users/{user.Id}", user);
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
            var token = await _userService.LoginAsync(dto);
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

        /// <summary>
        /// Retrieves information about the currently authenticated user.
        /// </summary>
        /// <returns>
        /// Returns the username and email of the authenticated user with status code 200,
        /// or 401 if the user is not authenticated.
        /// </returns>
        [HttpGet("me")]
        [Authorize]
        [ProducesResponseType(typeof(object), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<ActionResult<User>> Me()
        {
            var userIdClaim = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(userIdClaim)) return Unauthorized();

            if (!Guid.TryParse(userIdClaim, out var userId)) return Unauthorized();

            var user = await _userService.GetByIdAsync(userId);
            if (user == null) return Unauthorized();

            return Ok(user);
        }

        /// <summary>
        /// Partially updates the authenticated user's own profile.
        /// </summary>
        /// <param name="id">The unique identifier of the user.</param>
        /// <param name="dto">The fields to update (only non-null values will be applied).</param>
        /// <returns>
        /// Returns:
        /// - <see cref="StatusCodes.Status200OK"/> with the updated <see cref="User"/> if successful.
        /// - <see cref="StatusCodes.Status404NotFound"/> if the user does not exist.
        /// - <see cref="StatusCodes.Status400BadRequest"/> if validation fails.
        /// - <see cref="StatusCodes.Status401Unauthorized"/> if the request has no valid JWT.
        /// - <see cref="StatusCodes.Status403Forbidden"/> if the authenticated user tries to update another user's profile.
        /// </returns>
        [HttpPatch("{id:guid}")]
        [Authorize]
        [ProducesResponseType(typeof(User), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(ValidationProblemDetails), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        public async Task<ActionResult<User>> Update(Guid id, [FromBody] UpdateUserDto dto)
        {
            var userIdClaim = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(userIdClaim)) return Unauthorized();

            if (!Guid.TryParse(userIdClaim, out var authenticatedUserId)) return Unauthorized();

            if (authenticatedUserId != id) return Forbid();

            var updatedUser = await _userService.PatchAsync(id, dto);
            if (updatedUser == null) return NotFound();

            return Ok(updatedUser);
        }



        /// <summary>
        /// Deletes a user by its unique identifier.
        /// </summary>
        /// <param name="id">
        /// The unique identifier of the user to be deleted.
        /// </param>
        /// <returns>
        /// Returns:
        /// - <see cref="StatusCodes.Status204NoContent"/> if the user was successfully deleted.
        /// - <see cref="StatusCodes.Status404NotFound"/> if the user does not exist.
        /// - <see cref="ValidationProblemDetails"/> with <see cref="StatusCodes.Status400BadRequest"/> 
        ///   if the provided <paramref name="id"/> is invalid.
        /// </returns>
        [HttpDelete("{id:guid}")]
        [Authorize]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(ValidationProblemDetails), StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> Delete(Guid id)
        {
            var result = await _userService.DeleteAsync(id);

            if (result == null)
                return NotFound();

            return NoContent();
        }
    }
}
