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
    public class UsersController_Update_Tests
    {
        [Fact]
        public async Task Should_Propagate_UserValidationException_When_Profile_Update_Is_Invalid()
        {
            // Arrange
            var userRepository = new Mock<IUserRepository>();
            var user = new User("existinguser", "existing@example.com", "hash");
            userRepository.Setup(repository => repository.FindAsync(It.IsAny<Dictionary<string, object>>()))
                .ReturnsAsync(user);
            var service = new UserService(
                userRepository.Object,
                Options.Create(new JwtSettings
                {
                    Secret = "TestSecret",
                    Issuer = "TestIssuer",
                    Audience = "TestAudience"
                }));
            var currentUser = new Mock<ICurrentUser>();
            currentUser.SetupGet(current => current.UserId).Returns(user.Id);
            var controller = new UsersController(service, currentUser.Object);

            // Act
            var exception = await Assert.ThrowsAsync<UserValidationException>(
                () => controller.Update(new UpdateUserDto { Username = " " }));

            // Assert
            Assert.Contains(nameof(UpdateUserDto.Username), exception.Errors.Keys);
            userRepository.Verify(repository => repository.PatchAsync(It.IsAny<User>()), Times.Never);
        }

        [Fact]
        public async Task Should_Propagate_UserAlreadyExistsException_When_Profile_Update_Creates_Duplicate()
        {
            // Arrange
            var userRepository = new Mock<IUserRepository>();
            var user = new User("existinguser", "existing@example.com", "hash");
            userRepository.Setup(repository => repository.FindAsync(It.IsAny<Dictionary<string, object>>()))
                .ReturnsAsync(user);
            userRepository.Setup(repository => repository.PatchAsync(It.IsAny<User>()))
                .ThrowsAsync(new UserAlreadyExistsException());
            var service = new UserService(
                userRepository.Object,
                Options.Create(new JwtSettings
                {
                    Secret = "TestSecret",
                    Issuer = "TestIssuer",
                    Audience = "TestAudience"
                }));
            var currentUser = new Mock<ICurrentUser>();
            currentUser.SetupGet(current => current.UserId).Returns(user.Id);
            var controller = new UsersController(service, currentUser.Object);

            // Act
            var exception = await Assert.ThrowsAsync<UserAlreadyExistsException>(
                () => controller.Update(new UpdateUserDto { Username = "anotheruser" }));

            // Assert
            Assert.Equal("Username or email is already in use.", exception.Message);
        }

    }
}
