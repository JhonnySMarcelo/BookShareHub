using BookShareHub.Application.Configurations;
using BookShareHub.Application.Users.DTOs;
using BookShareHub.Application.Users.Services;
using BookShareHub.Domain.Users.Entities;
using BookShareHub.Domain.Users.Repositories;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;
using Moq;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;

namespace BookShareHub.Tests.Application.Users.UserServiceTests
{
    public class UserService_LoginAsync_Tests
    {
        private readonly Mock<IUserRepository> _userRepoMock;
        private readonly UserService _service;

        public UserService_LoginAsync_Tests()
        {
            _userRepoMock = new Mock<IUserRepository>();

            var jwtSettings = Options.Create(new JwtSettings
            {
                Secret = "SuperSecretKeyForTests1234561871877478487",
                Issuer = "TestIssuer",
                Audience = "TestAudience"
            });

            _service = new UserService(_userRepoMock.Object, jwtSettings);
        }

        [Fact]
        public async Task Should_Return_Null_When_User_Not_Found()
        {
            // Arrange
            var dto = new LoginUserDto { Email = "missing@example.com", Password = "password" };
            _userRepoMock.Setup(r => r.FindAsync(It.IsAny<Dictionary<string, object>>()))
                         .ReturnsAsync((User?)null);

            // Act
            var result = await _service.LoginAsync(dto);

            // Assert
            Assert.Null(result);
        }

        [Fact]
        public async Task Should_Return_Null_When_Password_Is_Invalid()
        {
            // Arrange
            var user = new User("jhonny", "jhonny@example.com", "hash");
            var passwordHasher = new PasswordHasher<User>();
            user.UpdatePasswordHash(passwordHasher.HashPassword(user, "CorrectPassword"));

            var dto = new LoginUserDto { Email = user.Email, Password = "WrongPassword" };

            _userRepoMock.Setup(r => r.FindAsync(It.IsAny<Dictionary<string, object>>()))
                         .ReturnsAsync(user);

            // Act
            var result = await _service.LoginAsync(dto);

            // Assert
            Assert.Null(result);
        }

        [Fact]
        public async Task Should_Return_JwtToken_When_Login_Is_Successful()
        {
            // Arrange
            var user = new User("jhonny", "jhonny@example.com", "hash");
            var passwordHasher = new PasswordHasher<User>();
            user.UpdatePasswordHash(passwordHasher.HashPassword(user, "CorrectPassword"));

            var dto = new LoginUserDto { Email = user.Email, Password = "CorrectPassword" };

            _userRepoMock.Setup(r => r.FindAsync(It.IsAny<Dictionary<string, object>>()))
                         .ReturnsAsync(user);

            // Act
            var token = await _service.LoginAsync(dto);

            // Assert
            Assert.NotNull(token);

            var handler = new JwtSecurityTokenHandler();
            var jwt = handler.ReadJwtToken(token!);

            Assert.Equal("TestIssuer", jwt.Issuer);
            Assert.Equal("TestAudience", jwt.Audiences.First());

            // Claims validation with URIs
            Assert.Contains(jwt.Claims, c => c.Type == ClaimTypes.NameIdentifier && c.Value == user.Id.ToString());
            Assert.Contains(jwt.Claims, c => c.Type == ClaimTypes.Email && c.Value == user.Email);
            Assert.Contains(jwt.Claims, c => c.Type == ClaimTypes.Name && c.Value == user.Username);
            Assert.Contains(jwt.Claims, c => c.Type == ClaimTypes.Role && c.Value == "User");
        }
    }
}
