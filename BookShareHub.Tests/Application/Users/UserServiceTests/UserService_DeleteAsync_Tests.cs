using BookShareHub.Application.Configurations;
using BookShareHub.Application.Users.Services;
using BookShareHub.Domain.Users.Entities;
using BookShareHub.Domain.Users.Repositories;
using Moq;
using Microsoft.Extensions.Options;

namespace BookShareHub.Tests.Application.Users.UserServiceTests
{
    public class UserService_DeleteAsync_Tests
    {
        private readonly Mock<IUserRepository> _userRepoMock;
        private readonly UserService _service;

        public UserService_DeleteAsync_Tests()
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
        public async Task Should_Return_True_When_User_Is_Deleted()
        {
            // Arrange
            var user = new User("jhonny", "jhonny@example.com", "hash");
            _userRepoMock.Setup(r => r.FindAsync(It.IsAny<Dictionary<string, object>>()))
                         .ReturnsAsync(user);
            _userRepoMock.Setup(r => r.DeleteAsync(user.Id)).ReturnsAsync(true);

            // Act
            var result = await _service.DeleteAsync(user.Id);

            // Assert
            Assert.True(result);
            _userRepoMock.Verify(r => r.DeleteAsync(user.Id), Times.Once);
        }

        [Fact]
        public async Task Should_Return_Null_When_User_Not_Found()
        {
            // Arrange
            var id = Guid.NewGuid();
            _userRepoMock.Setup(r => r.FindAsync(It.IsAny<Dictionary<string, object>>()))
                         .ReturnsAsync((User?)null);

            // Act
            var result = await _service.DeleteAsync(id);

            // Assert
            Assert.Null(result);
            _userRepoMock.Verify(r => r.DeleteAsync(It.IsAny<Guid>()), Times.Never);
        }


        [Fact]
        public async Task Should_Throw_ArgumentException_When_Id_Is_Empty()
        {
            // Arrange
            var emptyId = Guid.Empty;

            // Act & Assert
            await Assert.ThrowsAsync<ArgumentException>(() => _service.DeleteAsync(emptyId));

            _userRepoMock.Verify(r => r.FindAsync(It.IsAny<Dictionary<string, object>>()), Times.Never);
            _userRepoMock.Verify(r => r.DeleteAsync(It.IsAny<Guid>()), Times.Never);
        }
    }
}
