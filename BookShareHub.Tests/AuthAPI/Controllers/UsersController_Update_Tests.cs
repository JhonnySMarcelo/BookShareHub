using BookShareHub.Application.Configurations;
using BookShareHub.Application.Users.DTOs.Requests;
using BookShareHub.Application.Users.Exceptions;
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
    public class UsersController_Update_Tests
    {
        [Fact]
        public async Task Should_Return_400_ValidationProblemDetails_When_Profile_Update_Is_Invalid()
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
            var result = await controller.Update(new UpdateUserDto { Username = " " });

            // Assert
            var badRequest = Assert.IsType<BadRequestObjectResult>(result.Result);
            var problem = Assert.IsType<ValidationProblemDetails>(badRequest.Value);
            Assert.Contains(nameof(UpdateUserDto.Username), problem.Errors.Keys);
            userRepository.Verify(repository => repository.PatchAsync(It.IsAny<User>()), Times.Never);
        }

        [Fact]
        public async Task Should_Return_409_ProblemDetails_When_Profile_Update_Creates_Duplicate()
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
            var result = await controller.Update(new UpdateUserDto { Username = "anotheruser" });

            // Assert
            var conflict = Assert.IsType<ConflictObjectResult>(result.Result);
            Assert.Equal(StatusCodes.Status409Conflict, conflict.StatusCode);

            var problem = Assert.IsType<ProblemDetails>(conflict.Value);
            Assert.Equal("about:blank", problem.Type);
            Assert.Equal("Conflict", problem.Title);
            Assert.Equal(StatusCodes.Status409Conflict, problem.Status);
            Assert.Equal("Username or email is already in use.", problem.Detail);
        }

    }
}
