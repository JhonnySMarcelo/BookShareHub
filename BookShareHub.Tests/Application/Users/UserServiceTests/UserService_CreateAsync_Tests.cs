using BookShareHub.Application.Configurations;
using BookShareHub.Application.Users.Services;
using BookShareHub.Application.Users.DTOs;
using BookShareHub.Domain.Users.Entities;
using BookShareHub.Domain.Users.Repositories;
using Moq;
using Microsoft.Extensions.Options;

namespace BookShareHub.Tests.Application.Users.UserServiceTests
{
    public class UserService_CreateAsync_Tests
    {
        private readonly Mock<IUserRepository> _userRepoMock;
        private readonly UserService _service;

        public UserService_CreateAsync_Tests()
        {
            _userRepoMock = new Mock<IUserRepository>();

            // Cria JwtSettings fake só para satisfazer o construtor
            var jwtSettings = Options.Create(new JwtSettings
            {
                Secret = "TestSecret",
                Issuer = "TestIssuer",
                Audience = "TestAudience"
            });

            _service = new UserService(_userRepoMock.Object, jwtSettings);
        }

        [Fact]
        public async Task Should_Create_User_When_Valid()
        {
            // Arrange
            var dto = new RegisterUserDto
            {
                Username = "jhonny",
                Email = "jhonny@example.com",
                Password = "StrongPassword123"
            };

            _userRepoMock.Setup(r => r.GetByEmailAsync(dto.Email))
                         .ReturnsAsync((User?)null);
            _userRepoMock.Setup(r => r.AddAsync(It.IsAny<User>()))
                         .Returns(Task.CompletedTask);

            // Act
            var result = await _service.CreateAsync(dto);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(dto.Username, result.Username);
            Assert.Equal(dto.Email, result.Email);
            _userRepoMock.Verify(r => r.AddAsync(It.IsAny<User>()), Times.Once);
        }

        [Fact]
        public async Task Should_Throw_InvalidOperationException_When_Email_Already_Exists()
        {
            // Arrange
            var dto = new RegisterUserDto
            {
                Username = "jhonny",
                Email = "jhonny@example.com",
                Password = "StrongPassword123"
            };

            var existingUser = new User(dto.Username, dto.Email, "hash");
            _userRepoMock.Setup(r => r.GetByEmailAsync(dto.Email))
                         .ReturnsAsync(existingUser);

            // Act & Assert
            await Assert.ThrowsAsync<InvalidOperationException>(() => _service.CreateAsync(dto));

            _userRepoMock.Verify(r => r.AddAsync(It.IsAny<User>()), Times.Never);
        }

        [Theory]
        [InlineData("")]
        [InlineData("   ")]
        public async Task Should_Throw_ArgumentException_When_Username_Is_Invalid(string invalidUsername)
        {
            // Arrange
            var dto = new RegisterUserDto
            {
                Username = invalidUsername,
                Email = "valid@example.com",
                Password = "StrongPassword123"
            };

            _userRepoMock.Setup(r => r.GetByEmailAsync(dto.Email))
                         .ReturnsAsync((User?)null);

            // Act & Assert
            await Assert.ThrowsAsync<ArgumentException>(() => _service.CreateAsync(dto));

            _userRepoMock.Verify(r => r.AddAsync(It.IsAny<User>()), Times.Never);
        }

        [Theory]
        [InlineData("")]
        [InlineData("   ")]
        public async Task Should_Throw_ArgumentException_When_Email_Is_Invalid(string invalidEmail)
        {
            // Arrange
            var dto = new RegisterUserDto
            {
                Username = "validuser",
                Email = invalidEmail,
                Password = "StrongPassword123"
            };

            _userRepoMock.Setup(r => r.GetByEmailAsync(dto.Email))
                         .ReturnsAsync((User?)null);

            // Act & Assert
            await Assert.ThrowsAsync<ArgumentException>(() => _service.CreateAsync(dto));

            _userRepoMock.Verify(r => r.AddAsync(It.IsAny<User>()), Times.Never);
        }

        [Fact]
        public async Task Should_Hash_Password_Before_Saving()
        {
            // Arrange
            var dto = new RegisterUserDto
            {
                Username = "jhonny",
                Email = "jhonny@example.com",
                Password = "StrongPassword123"
            };

            _userRepoMock.Setup(r => r.GetByEmailAsync(dto.Email))
                         .ReturnsAsync((User?)null);
            _userRepoMock.Setup(r => r.AddAsync(It.IsAny<User>()))
                         .Returns(Task.CompletedTask);

            // Act
            var result = await _service.CreateAsync(dto);
            
            // Assert
            Assert.NotEqual("StrongPassword123", result.PasswordHash);
            Assert.False(string.IsNullOrWhiteSpace(result.PasswordHash));
        }

    }
}
