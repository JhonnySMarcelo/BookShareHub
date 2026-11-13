using BookShareHub.Application.Books.DTOs.Requests;
using BookShareHub.Application.Books.Exceptions;
using BookShareHub.BooksAPI.ExceptionHandling;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Moq;

namespace BookShareHub.Tests.BooksAPI.ExceptionHandling
{
    public class BooksApiExceptionHandler_Tests
    {
        [Fact]
        public async Task Should_Write_ValidationProblemDetails_With_Book_Field_Errors()
        {
            // Arrange
            var errors = new Dictionary<string, string[]>
            {
                ["id"] = ["Book Id cannot be empty."],
                [nameof(CreateBookRequest.Title)] = ["Book Title is required."]
            };
            ProblemDetailsContext? writtenContext = null;
            var problemDetailsService = new Mock<IProblemDetailsService>();
            problemDetailsService
                .Setup(service => service.TryWriteAsync(It.IsAny<ProblemDetailsContext>()))
                .Callback<ProblemDetailsContext>(context => writtenContext = context)
                .Returns(new ValueTask<bool>(true));
            var handler = new BooksApiExceptionHandler(problemDetailsService.Object);
            var httpContext = new DefaultHttpContext();
            var exception = new BookValidationException(errors);

            // Act
            var handled = await handler.TryHandleAsync(httpContext, exception, CancellationToken.None);

            // Assert
            Assert.True(handled);
            Assert.Equal(StatusCodes.Status400BadRequest, httpContext.Response.StatusCode);
            Assert.NotNull(writtenContext);
            var validationProblem = Assert.IsType<ValidationProblemDetails>(writtenContext.ProblemDetails);
            Assert.Equal(StatusCodes.Status400BadRequest, validationProblem.Status);
            Assert.Equal("One or more validation errors occurred.", validationProblem.Title);
            Assert.Equal(errors.Keys, validationProblem.Errors.Keys);
            foreach (var error in errors)
            {
                Assert.Equal(error.Value, validationProblem.Errors[error.Key]);
            }
        }

        [Fact]
        public async Task Should_Write_Conflict_ProblemDetails_For_BookUnavailable()
        {
            // Arrange
            ProblemDetailsContext? writtenContext = null;
            var problemDetailsService = new Mock<IProblemDetailsService>();
            problemDetailsService
                .Setup(service => service.TryWriteAsync(It.IsAny<ProblemDetailsContext>()))
                .Callback<ProblemDetailsContext>(context => writtenContext = context)
                .Returns(new ValueTask<bool>(true));
            var handler = new BooksApiExceptionHandler(problemDetailsService.Object);
            var httpContext = new DefaultHttpContext();
            var exception = new BookUnavailableException();

            // Act
            var handled = await handler.TryHandleAsync(httpContext, exception, CancellationToken.None);

            // Assert
            Assert.True(handled);
            Assert.Equal(StatusCodes.Status409Conflict, httpContext.Response.StatusCode);
            Assert.NotNull(writtenContext);
            var problemDetails = Assert.IsType<ProblemDetails>(writtenContext.ProblemDetails);
            Assert.Equal(StatusCodes.Status409Conflict, problemDetails.Status);
            Assert.Equal("urn:booksharehub:problem:book-unavailable", problemDetails.Type);
            Assert.Equal("Conflict", problemDetails.Title);
            Assert.Equal("Book is currently borrowed and cannot be deleted.", problemDetails.Detail);
        }

        [Fact]
        public async Task Should_Write_Sanitized_InternalServerError_For_Unhandled_Exception()
        {
            // Arrange
            ProblemDetailsContext? writtenContext = null;
            var problemDetailsService = new Mock<IProblemDetailsService>();
            problemDetailsService
                .Setup(service => service.TryWriteAsync(It.IsAny<ProblemDetailsContext>()))
                .Callback<ProblemDetailsContext>(context => writtenContext = context)
                .Returns(new ValueTask<bool>(true));
            var handler = new BooksApiExceptionHandler(problemDetailsService.Object);
            var httpContext = new DefaultHttpContext();
            var exception = new InvalidOperationException("database credentials leaked");

            // Act
            var handled = await handler.TryHandleAsync(httpContext, exception, CancellationToken.None);

            // Assert
            Assert.True(handled);
            Assert.Equal(StatusCodes.Status500InternalServerError, httpContext.Response.StatusCode);
            Assert.NotNull(writtenContext);
            var problemDetails = Assert.IsType<ProblemDetails>(writtenContext.ProblemDetails);
            Assert.Equal(StatusCodes.Status500InternalServerError, problemDetails.Status);
            Assert.Equal("about:blank", problemDetails.Type);
            Assert.Equal("Internal Server Error", problemDetails.Title);
            Assert.Equal("An unexpected error occurred.", problemDetails.Detail);
            Assert.DoesNotContain("database credentials leaked", problemDetails.Detail);
            Assert.Empty(problemDetails.Extensions);
        }
    }
}
