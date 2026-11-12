using BookShareHub.Application.Users.Exceptions;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc;

namespace BookShareHub.AuthAPI.ExceptionHandling
{
    /// <summary>
    /// Maps authentication API exceptions to Problem Details responses.
    /// </summary>
    public sealed class AuthApiExceptionHandler : IExceptionHandler
    {
        private readonly IProblemDetailsService _problemDetailsService;

        /// <summary>
        /// Initializes the handler with the registered Problem Details service.
        /// </summary>
        /// <param name="problemDetailsService">The service that writes Problem Details responses.</param>
        public AuthApiExceptionHandler(IProblemDetailsService problemDetailsService)
        {
            _problemDetailsService = problemDetailsService;
        }

        /// <inheritdoc />
        public async ValueTask<bool> TryHandleAsync(
            HttpContext httpContext,
            Exception exception,
            CancellationToken cancellationToken)
        {
            ProblemDetails problemDetails = exception switch
            {
                UserValidationException validationException => new ValidationProblemDetails(validationException.Errors)
                {
                    Status = StatusCodes.Status400BadRequest
                },
                UserAlreadyExistsException => new ProblemDetails
                {
                    Type = "urn:booksharehub:problem:user-already-exists",
                    Title = "Conflict",
                    Status = StatusCodes.Status409Conflict,
                    Detail = "Username or email is already in use."
                },
                _ => new ProblemDetails
                {
                    Type = "about:blank",
                    Title = "Internal Server Error",
                    Status = StatusCodes.Status500InternalServerError,
                    Detail = "An unexpected error occurred."
                }
            };

            httpContext.Response.StatusCode = problemDetails.Status!.Value;

            return await _problemDetailsService.TryWriteAsync(new ProblemDetailsContext
            {
                HttpContext = httpContext,
                Exception = exception,
                ProblemDetails = problemDetails
            });
        }
    }
}
