using BookShareHub.Application.Configurations;
using BookShareHub.Application.Users.DTOs.Requests;
using BookShareHub.Application.Users.Exceptions;
using BookShareHub.Application.Users.Interfaces;
using BookShareHub.Application.Users.Services;
using BookShareHub.Domain.Users.Entities;
using BookShareHub.Domain.Users.Repositories;
using BookShareHub.UsersAPI.Controllers;
using Microsoft.Extensions.Options;
using Moq;

namespace BookShareHub.Tests.AuthAPI.Controllers
{
    public class UsersController_Register_Tests
    {
        [Fact]
        public async Task Should_Propagate_UserAlreadyExistsException_When_User_Already_Exists()
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
            var exception = await Assert.ThrowsAsync<UserAlreadyExistsException>(() => controller.Register(request));

            // Assert
            Assert.Equal("Username or email is already in use.", exception.Message);
            userRepository.Verify(repository => repository.AddAsync(It.IsAny<User>()), Times.Never);
        }

        [Fact]
        public async Task Should_Propagate_UserValidationException_When_Registration_Is_Invalid()
        {
            // Arrange
            var service = new UserService(
                Mock.Of<IUserRepository>(),
                Options.Create(new JwtSettings
                {
                    Secret = "TestSecret",
                    Issuer = "TestIssuer",
                    Audience = "TestAudience"
                }));
            var controller = new UsersController(service, Mock.Of<ICurrentUser>());
            var request = new RegisterUserDto
            {
                Username = " ",
                Email = "new@example.com",
                Password = "StrongPassword123"
            };

            // Act
            var exception = await Assert.ThrowsAsync<UserValidationException>(() => controller.Register(request));

            // Assert
            Assert.Contains(nameof(request.Username), exception.Errors.Keys);
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
