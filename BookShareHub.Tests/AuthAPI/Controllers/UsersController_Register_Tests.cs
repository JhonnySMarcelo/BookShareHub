using BookShareHub.Application.Configurations;
using BookShareHub.Application.Users.DTOs.Requests;
using BookShareHub.Application.Users.Interfaces;
using BookShareHub.Application.Users.Services;
using BookShareHub.Domain.Users.Entities;
using BookShareHub.Domain.Users.Repositories;
using BookShareHub.UsersAPI.Controllers;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Moq;

namespace BookShareHub.Tests.AuthAPI.Controllers
{
    public class UsersController_Register_Tests
    {
        [Fact]
        public async Task Should_Return_409_ProblemDetails_When_User_Already_Exists()
        {
            // Arrange
            var userRepository = new Mock<IUserRepository>();
            userRepository.Setup(repository => repository.FindAsync(It.IsAny<Dictionary<string, object>>()))
                .ReturnsAsync(new User("existinguser", "existing@example.com", "hash"));

            var service = new UserService(
                userRepository.Object,
                Options.Create(new JwtSettings
                {
                    Secret = "TestSecret",
                    Issuer = "TestIssuer",
                    Audience = "TestAudience"
                }));
            var controller = new UsersController(service, Mock.Of<ICurrentUser>());
            var request = new RegisterUserDto
            {
                Username = "newuser",
                Email = "existing@example.com",
                Password = "StrongPassword123"
            };

            // Act
            var result = await controller.Register(request);

            // Assert
            var conflict = Assert.IsType<ConflictObjectResult>(result.Result);
            Assert.Equal(StatusCodes.Status409Conflict, conflict.StatusCode);

            var problem = Assert.IsType<ProblemDetails>(conflict.Value);
            Assert.Equal("about:blank", problem.Type);
            Assert.Equal("Conflict", problem.Title);
            Assert.Equal(StatusCodes.Status409Conflict, problem.Status);
            Assert.Equal("Username or email is already in use.", problem.Detail);

            userRepository.Verify(repository => repository.AddAsync(It.IsAny<User>()), Times.Never);
        }

        [Fact]
        public async Task Should_Propagate_Unrelated_InvalidOperationException()
        {
            // Arrange
            var userRepository = new Mock<IUserRepository>();
            userRepository.Setup(repository => repository.FindAsync(It.IsAny<Dictionary<string, object>>()))
                .ThrowsAsync(new InvalidOperationException("Unexpected repository failure."));

            var service = new UserService(
                userRepository.Object,
                Options.Create(new JwtSettings
                {
                    Secret = "TestSecret",
                    Issuer = "TestIssuer",
                    Audience = "TestAudience"
                }));
            var controller = new UsersController(service, Mock.Of<ICurrentUser>());
            var request = new RegisterUserDto
            {
                Username = "newuser",
                Email = "new@example.com",
                Password = "StrongPassword123"
            };

            // Act
            var exception = await Record.ExceptionAsync(() => controller.Register(request));

            // Assert
            Assert.IsType<InvalidOperationException>(exception);
            Assert.Equal("Unexpected repository failure.", exception.Message);
        }
    }
}
