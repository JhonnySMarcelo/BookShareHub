using BookShareHub.Application.Users.Exceptions;
using BookShareHub.AuthAPI.ExceptionHandling;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Moq;

namespace BookShareHub.Tests.AuthAPI.ExceptionHandling
{
    public class AuthApiExceptionHandler_Tests
    {
        [Fact]
        public async Task Should_Write_ValidationProblemDetails_With_User_Field_Errors()
        {
            // Arrange
            var errors = new Dictionary<string, string[]>
            {
                ["Username"] = ["Username is required."],
                ["Email"] = ["Email is invalid."]
            };
            ProblemDetailsContext? writtenContext = null;
            var problemDetailsService = new Mock<IProblemDetailsService>();
            problemDetailsService
                .Setup(service => service.TryWriteAsync(It.IsAny<ProblemDetailsContext>()))
                .Callback<ProblemDetailsContext>(context => writtenContext = context)
                .Returns(new ValueTask<bool>(true));
            var handler = new AuthApiExceptionHandler(problemDetailsService.Object);
            var httpContext = new DefaultHttpContext();
            var exception = new UserValidationException(errors);

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
        public async Task Should_Write_Conflict_ProblemDetails_For_UserAlreadyExists()
        {
            // Arrange
            ProblemDetailsContext? writtenContext = null;
            var problemDetailsService = new Mock<IProblemDetailsService>();
            problemDetailsService
                .Setup(service => service.TryWriteAsync(It.IsAny<ProblemDetailsContext>()))
                .Callback<ProblemDetailsContext>(context => writtenContext = context)
                .Returns(new ValueTask<bool>(true));
            var handler = new AuthApiExceptionHandler(problemDetailsService.Object);
            var httpContext = new DefaultHttpContext();
            var exception = new UserAlreadyExistsException();

            // Act
            var handled = await handler.TryHandleAsync(httpContext, exception, CancellationToken.None);

            // Assert
            Assert.True(handled);
            Assert.Equal(StatusCodes.Status409Conflict, httpContext.Response.StatusCode);
            Assert.NotNull(writtenContext);
            var problemDetails = Assert.IsType<ProblemDetails>(writtenContext.ProblemDetails);
            Assert.Equal(StatusCodes.Status409Conflict, problemDetails.Status);
            Assert.Equal("urn:booksharehub:problem:user-already-exists", problemDetails.Type);
            Assert.Equal("Conflict", problemDetails.Title);
            Assert.Equal("Username or email is already in use.", problemDetails.Detail);
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
            var handler = new AuthApiExceptionHandler(problemDetailsService.Object);
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
