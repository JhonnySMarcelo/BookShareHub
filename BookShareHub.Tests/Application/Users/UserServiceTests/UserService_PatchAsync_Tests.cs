using BookShareHub.Application.Configurations;
using BookShareHub.Application.Users.Services;
using BookShareHub.Application.Users.DTOs;
using BookShareHub.Domain.Users.Entities;
using BookShareHub.Domain.Users.Repositories;
using Moq;
using Microsoft.Extensions.Options;

namespace BookShareHub.Tests.Application.Users.UserServiceTests
{
    public class UserService_PatchAsync_Tests
    {
        private readonly Mock<IUserRepository> _userRepoMock;
        private readonly UserService _service;

        public UserService_PatchAsync_Tests()
        {
            _userRepoMock = new Mock<IUserRepository>();

            var jwtSettings = Options.Create(new JwtSettings
            {
                Secret = "TestSecret",
                Issuer = "TestIssuer",
                Audience = "TestAudience"
            });

            _service = new UserService(_userRepoMock.Object, jwtSettings);
        }

        [Fact]
        public async Task Should_Throw_ArgumentException_When_Id_Is_Empty()
        {
            // Arrange
            var emptyId = Guid.Empty;
            var dto = new UpdateUserDto { Username = "newname" };

            // Act & Assert
            await Assert.ThrowsAsync<ArgumentException>(() => _service.PatchAsync(emptyId, dto));

            _userRepoMock.Verify(r => r.FindAsync(It.IsAny<Dictionary<string, object>>()), Times.Never);
            _userRepoMock.Verify(r => r.PatchAsync(It.IsAny<User>()), Times.Never);
        }

        [Fact]
        public async Task Should_Throw_ArgumentNullException_When_Dto_Is_Null()
        {
            // Arrange
            var id = Guid.NewGuid();

            // Act & Assert
            await Assert.ThrowsAsync<ArgumentNullException>(() => _service.PatchAsync(id, null!));

            _userRepoMock.Verify(r => r.FindAsync(It.IsAny<Dictionary<string, object>>()), Times.Never);
            _userRepoMock.Verify(r => r.PatchAsync(It.IsAny<User>()), Times.Never);
        }

        [Fact]
        public async Task Should_Return_Null_When_User_Not_Found()
        {
            // Arrange
            var id = Guid.NewGuid();
            var dto = new UpdateUserDto { Username = "newname" };

            _userRepoMock.Setup(r => r.FindAsync(It.IsAny<Dictionary<string, object>>()))
                         .ReturnsAsync((User?)null);

            // Act
            var result = await _service.PatchAsync(id, dto);

            // Assert
            Assert.Null(result);
            _userRepoMock.Verify(r => r.PatchAsync(It.IsAny<User>()), Times.Never);
        }

        [Fact]
        public async Task Should_Update_Username_When_Provided()
        {
            // Arrange
            var user = new User("oldname", "user@example.com", "hash");
            var dto = new UpdateUserDto { Username = "newname" };

            _userRepoMock.Setup(r => r.FindAsync(It.IsAny<Dictionary<string, object>>()))
                         .ReturnsAsync(user);
            _userRepoMock.Setup(r => r.PatchAsync(It.IsAny<User>()))
                         .ReturnsAsync(user);

            // Act
            var result = await _service.PatchAsync(user.Id, dto);

            // Assert
            Assert.NotNull(result);
            Assert.Equal("newname", result!.Username);
            _userRepoMock.Verify(r => r.PatchAsync(It.Is<User>(u => u.Username == "newname")), Times.Once);
        }

        [Fact]
        public async Task Should_Update_Email_When_Provided()
        {
            // Arrange
            var user = new User("jhonny", "old@example.com", "hash");
            var dto = new UpdateUserDto { Email = "new@example.com" };

            _userRepoMock.Setup(r => r.FindAsync(It.IsAny<Dictionary<string, object>>()))
                         .ReturnsAsync(user);
            _userRepoMock.Setup(r => r.PatchAsync(It.IsAny<User>()))
                         .ReturnsAsync(user);

            // Act
            var result = await _service.PatchAsync(user.Id, dto);

            // Assert
            Assert.NotNull(result);
            Assert.Equal("new@example.com", result!.Email);
            _userRepoMock.Verify(r => r.PatchAsync(It.Is<User>(u => u.Email == "new@example.com")), Times.Once);
        }

        [Fact]
        public async Task Should_Update_Password_When_Provided()
        {
            // Arrange
            var user = new User("jhonny", "user@example.com", "oldhash");
            var dto = new UpdateUserDto { Password = "NewPassword123" };

            _userRepoMock.Setup(r => r.FindAsync(It.IsAny<Dictionary<string, object>>()))
                         .ReturnsAsync(user);
            _userRepoMock.Setup(r => r.PatchAsync(It.IsAny<User>()))
                         .ReturnsAsync(user);

            // Act
            var result = await _service.PatchAsync(user.Id, dto);

            // Assert
            Assert.NotNull(result);
            Assert.NotEqual("oldhash", result!.PasswordHash);
            Assert.False(string.IsNullOrWhiteSpace(result.PasswordHash));
            _userRepoMock.Verify(r => r.PatchAsync(It.Is<User>(u => u.PasswordHash != "oldhash")), Times.Once);
        }

        [Fact]
        public async Task Should_Update_All_Fields_When_All_Provided()
        {
            // Arrange
            var user = new User("oldname", "old@example.com", "oldhash");
            var dto = new UpdateUserDto
            {
                Username = "newname",
                Email = "new@example.com",
                Password = "NewPassword123"
            };

            _userRepoMock.Setup(r => r.FindAsync(It.IsAny<Dictionary<string, object>>()))
                         .ReturnsAsync(user);
            _userRepoMock.Setup(r => r.PatchAsync(It.IsAny<User>()))
                         .ReturnsAsync(user);

            // Act
            var result = await _service.PatchAsync(user.Id, dto);

            // Assert
            Assert.NotNull(result);
            Assert.Equal("newname", result!.Username);
            Assert.Equal("new@example.com", result.Email);
            Assert.NotEqual("oldhash", result.PasswordHash);
            _userRepoMock.Verify(r => r.PatchAsync(It.Is<User>(u =>
                u.Username == "newname" &&
                u.Email == "new@example.com" &&
                u.PasswordHash != "oldhash")), Times.Once);
        }
    }
}
