using BookShareHub.Application.Books.Exceptions;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc;

namespace BookShareHub.BooksAPI.ExceptionHandling
{
    /// <summary>
    /// Maps books API exceptions to Problem Details responses.
    /// </summary>
    public sealed class BooksApiExceptionHandler : IExceptionHandler
    {
        private readonly IProblemDetailsService _problemDetailsService;

        /// <summary>
        /// Initializes the handler with the registered Problem Details service.
        /// </summary>
        /// <param name="problemDetailsService">The service that writes Problem Details responses.</param>
        public BooksApiExceptionHandler(IProblemDetailsService problemDetailsService)
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
                BookUnavailableException => new ProblemDetails
                {
                    Type = "urn:booksharehub:problem:book-unavailable",
                    Title = "Conflict",
                    Status = StatusCodes.Status409Conflict,
                    Detail = "Book is currently borrowed and cannot be deleted."
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
