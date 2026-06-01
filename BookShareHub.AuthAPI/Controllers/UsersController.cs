using BookShareHub.Application.Users.DTOs;
using BookShareHub.Application.Users.Interfaces;
using BookShareHub.Application.Users.Services;
using BookShareHub.Domain.Users.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

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
        private readonly ICurrentUser _currentUser;

        /// <summary>
        /// Initializes a new instance of the <see cref="UsersController"/>.
        /// </summary>
        /// <param name="userService">The service responsible for user operations.</param>
        /// <param name="currentUser">Provides access to information about the currently authenticated user.</param>
        public UsersController(UserService userService, ICurrentUser currentUser)
        {
            _userService = userService;
            _currentUser = currentUser;
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
            var user = await _userService.GetByIdAsync(_currentUser.UserId);
            if (user == null) return Unauthorized();

            return Ok(user);
        }

        /// <summary>
        /// Partially updates the authenticated user's own profile.
        /// </summary>
        /// <param name="dto">The fields to update (only non-null values will be applied).</param>
        /// <returns>
        /// Returns:
        /// - <see cref="StatusCodes.Status200OK"/> with the updated <see cref="User"/> if successful.
        /// - <see cref="StatusCodes.Status404NotFound"/> if the user does not exist.
        /// - <see cref="StatusCodes.Status400BadRequest"/> if validation fails.
        /// - <see cref="StatusCodes.Status401Unauthorized"/> if the request has no valid JWT.
        /// - <see cref="StatusCodes.Status403Forbidden"/> if the authenticated user tries to update another user's profile.
        /// </returns>
        [HttpPatch("me")]
        [Authorize]
        [ProducesResponseType(typeof(User), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(ValidationProblemDetails), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        public async Task<ActionResult<User>> Update([FromBody] UpdateUserDto dto)
        {
            var updatedUser = await _userService.PatchAsync(_currentUser.UserId, dto);
            if (updatedUser == null) return NotFound();

            return Ok(updatedUser);
        }



        /// <summary>
        /// Deletes the currently authenticated user.
        /// </summary>
        /// <returns>
        /// Returns:
        /// - <see cref="StatusCodes.Status204NoContent"/> if the user was successfully deleted.
        /// - <see cref="StatusCodes.Status404NotFound"/> if the user does not exist.
        /// - <see cref="ValidationProblemDetails"/> with <see cref="StatusCodes.Status400BadRequest"/> 
        /// </returns>
        [HttpDelete("me")]
        [Authorize]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(ValidationProblemDetails), StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> Delete()
        {
            var result = await _userService.DeleteAsync(_currentUser.UserId);

            if (result == null)
                return NotFound();

            return NoContent();
        }
    }
}
