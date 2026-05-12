using BookShareHub.Application.Configurations;
using BookShareHub.Application.Users.Services;
using BookShareHub.Domain.Users.Entities;
using BookShareHub.Domain.Users.Repositories;
using Moq;
using Microsoft.Extensions.Options;

namespace BookShareHub.Tests.Application.Users.UserServiceTests
{
    public class UserService_GetByIdAsync_Tests
    {
        private readonly Mock<IUserRepository> _userRepoMock;
        private readonly UserService _service;

        public UserService_GetByIdAsync_Tests()
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

            // Act & Assert
            await Assert.ThrowsAsync<ArgumentException>(() => _service.GetByIdAsync(emptyId));

            _userRepoMock.Verify(r => r.FindAsync(It.IsAny<Dictionary<string, object>>()), Times.Never);
        }

        [Fact]
        public async Task Should_Return_User_When_Found()
        {
            // Arrange
            var user = new User("jhonny", "jhonny@example.com", "hash");
            _userRepoMock.Setup(r => r.FindAsync(It.IsAny<Dictionary<string, object>>()))
                         .ReturnsAsync(user);

            // Act
            var result = await _service.GetByIdAsync(user.Id);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(user.Id, result!.Id);
            Assert.Equal(user.Email, result.Email);
            Assert.Equal(user.Username, result.Username);
            _userRepoMock.Verify(r => r.FindAsync(It.IsAny<Dictionary<string, object>>()), Times.Once);
        }

        [Fact]
        public async Task Should_Return_Null_When_User_Not_Found()
        {
            // Arrange
            var id = Guid.NewGuid();
            _userRepoMock.Setup(r => r.FindAsync(It.IsAny<Dictionary<string, object>>()))
                         .ReturnsAsync((User?)null);

            // Act
            var result = await _service.GetByIdAsync(id);

            // Assert
            Assert.Null(result);
            _userRepoMock.Verify(r => r.FindAsync(It.IsAny<Dictionary<string, object>>()), Times.Once);
        }
    }
}
