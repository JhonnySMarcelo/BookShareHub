using BookShareHub.Application.Configurations;
using BookShareHub.Application.Users.Services;
using BookShareHub.Application.Users.DTOs.Requests;
using BookShareHub.Application.Users.Exceptions;
using BookShareHub.Domain.Users.Entities;
using BookShareHub.Domain.Users.Repositories;
using Moq;
using Microsoft.Extensions.Options;
using Microsoft.AspNetCore.Identity;

namespace BookShareHub.Tests.Application.Users.UserServiceTests
{
    public class UserService_CreateAsync_Tests
    {
        private readonly Mock<IUserRepository> _userRepoMock;
        private readonly UserService _service;

        public UserService_CreateAsync_Tests()
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
        public async Task Should_Create_User_When_Valid()
        {
            // Arrange
            var dto = new RegisterUserDto
            {
                Username = " jhonny ",
                Email = " jhonny@example.com ",
                Password = "StrongPassword123"
            };

            _userRepoMock.Setup(r => r.FindAsync(It.IsAny<Dictionary<string, object>>()))
                         .ReturnsAsync((User?)null);
            User? savedUser = null;
            _userRepoMock.Setup(r => r.AddAsync(It.IsAny<User>()))
                         .Callback<User>(user => savedUser = user)
                         .Returns(Task.CompletedTask);

            // Act
            var result = await _service.CreateAsync(dto);

            // Assert
            Assert.NotNull(result);
            Assert.Equal("jhonny", result.Username);
            Assert.Equal("jhonny@example.com", result.Email);
            Assert.NotNull(savedUser);
            Assert.Equal(savedUser.Id, result.Id);
            Assert.Equal(savedUser.CreationTime, result.CreationTime);
            Assert.Null(result.GetType().GetProperty(nameof(User.PasswordHash)));
            _userRepoMock.Verify(r => r.FindAsync(It.Is<Dictionary<string, object>>(criteria =>
                Equals(criteria["Username"], "jhonny") &&
                Equals(criteria["Email"], "jhonny@example.com"))), Times.Once);
            _userRepoMock.Verify(r => r.AddAsync(It.IsAny<User>()), Times.Once);
        }

        [Fact]
        public async Task Should_Throw_UserAlreadyExistsException_When_Email_Already_Exists()
        {
            // Arrange
            var dto = new RegisterUserDto
            {
                Username = "newuser",
                Email = "jhonny@example.com",
                Password = "StrongPassword123"
            };

            var existingUser = new User("otheruser", dto.Email, "hash");
            _userRepoMock.Setup(r => r.FindAsync(It.IsAny<Dictionary<string, object>>()))
                         .ReturnsAsync(existingUser);

            // Act
            var exception = await Record.ExceptionAsync(() => _service.CreateAsync(dto));

            // Assert
            Assert.IsType<UserAlreadyExistsException>(exception);
            Assert.Equal("Username or email is already in use.", exception.Message);
            _userRepoMock.Verify(r => r.AddAsync(It.IsAny<User>()), Times.Never);
        }

        [Fact]
        public async Task Should_Throw_UserAlreadyExistsException_When_Username_Already_Exists()
        {
            // Arrange
            var dto = new RegisterUserDto
            {
                Username = "jhonny",
                Email = "new@example.com",
                Password = "StrongPassword123"
            };

            var existingUser = new User(dto.Username, "other@example.com", "hash");
            _userRepoMock.Setup(r => r.FindAsync(It.IsAny<Dictionary<string, object>>()))
                         .ReturnsAsync(existingUser);

            // Act
            var exception = await Record.ExceptionAsync(() => _service.CreateAsync(dto));

            // Assert
            Assert.IsType<UserAlreadyExistsException>(exception);
            Assert.Equal("Username or email is already in use.", exception.Message);
            _userRepoMock.Verify(r => r.AddAsync(It.IsAny<User>()), Times.Never);
        }

        [Theory]
        [InlineData("")]
        [InlineData("   ")]
        public async Task Should_Throw_UserValidationException_When_Username_Is_Invalid(string invalidUsername)
        {
            // Arrange
            var dto = new RegisterUserDto
            {
                Username = invalidUsername,
                Email = "valid@example.com",
                Password = "StrongPassword123"
            };

            // Act
            var exception = await Assert.ThrowsAsync<UserValidationException>(() => _service.CreateAsync(dto));

            // Assert
            Assert.Contains(nameof(dto.Username), exception.Errors.Keys);
            _userRepoMock.Verify(r => r.FindAsync(It.IsAny<Dictionary<string, object>>()), Times.Never);
            _userRepoMock.Verify(r => r.AddAsync(It.IsAny<User>()), Times.Never);
        }

        [Fact]
        public async Task Should_Throw_UserValidationException_When_Username_Exceeds_Maximum_Length()
        {
            // Arrange
            var dto = new RegisterUserDto
            {
                Username = new string('u', 101),
                Email = "valid@example.com",
                Password = "StrongPassword123"
            };

            // Act
            var exception = await Assert.ThrowsAsync<UserValidationException>(() => _service.CreateAsync(dto));

            // Assert
            Assert.Contains(nameof(dto.Username), exception.Errors.Keys);
            _userRepoMock.Verify(r => r.FindAsync(It.IsAny<Dictionary<string, object>>()), Times.Never);
            _userRepoMock.Verify(r => r.AddAsync(It.IsAny<User>()), Times.Never);
        }

        [Theory]
        [InlineData("")]
        [InlineData("   ")]
        public async Task Should_Throw_UserValidationException_When_Email_Is_Required(string invalidEmail)
        {
            // Arrange
            var dto = new RegisterUserDto
            {
                Username = "validuser",
                Email = invalidEmail,
                Password = "StrongPassword123"
            };

            // Act
            var exception = await Assert.ThrowsAsync<UserValidationException>(() => _service.CreateAsync(dto));

            // Assert
            Assert.Contains(nameof(dto.Email), exception.Errors.Keys);
            _userRepoMock.Verify(r => r.FindAsync(It.IsAny<Dictionary<string, object>>()), Times.Never);
            _userRepoMock.Verify(r => r.AddAsync(It.IsAny<User>()), Times.Never);
        }

        [Theory]
        [InlineData("not-an-email")]
        [InlineData("Display Name <user@example.com>")]
        public async Task Should_Throw_UserValidationException_When_Email_Format_Is_Invalid(string invalidEmail)
        {
            // Arrange
            var dto = new RegisterUserDto
            {
                Username = "validuser",
                Email = invalidEmail,
                Password = "StrongPassword123"
            };

            // Act
            var exception = await Assert.ThrowsAsync<UserValidationException>(() => _service.CreateAsync(dto));

            // Assert
            Assert.Contains(nameof(dto.Email), exception.Errors.Keys);
            _userRepoMock.Verify(r => r.FindAsync(It.IsAny<Dictionary<string, object>>()), Times.Never);
            _userRepoMock.Verify(r => r.AddAsync(It.IsAny<User>()), Times.Never);
        }

        [Fact]
        public async Task Should_Throw_UserValidationException_When_Email_Exceeds_Maximum_Length()
        {
            // Arrange
            var dto = new RegisterUserDto
            {
                Username = "validuser",
                Email = $"{new string('a', 189)}@example.com",
                Password = "StrongPassword123"
            };

            // Act
            var exception = await Assert.ThrowsAsync<UserValidationException>(() => _service.CreateAsync(dto));

            // Assert
            Assert.Contains(nameof(dto.Email), exception.Errors.Keys);
            _userRepoMock.Verify(r => r.FindAsync(It.IsAny<Dictionary<string, object>>()), Times.Never);
            _userRepoMock.Verify(r => r.AddAsync(It.IsAny<User>()), Times.Never);
        }

        [Theory]
        [InlineData("")]
        [InlineData("   ")]
        public async Task Should_Throw_UserValidationException_When_Password_Is_Required(string invalidPassword)
        {
            // Arrange
            var dto = new RegisterUserDto
            {
                Username = "validuser",
                Email = "valid@example.com",
                Password = invalidPassword
            };

            // Act
            var exception = await Assert.ThrowsAsync<UserValidationException>(() => _service.CreateAsync(dto));

            // Assert
            Assert.Contains(nameof(dto.Password), exception.Errors.Keys);
            _userRepoMock.Verify(r => r.FindAsync(It.IsAny<Dictionary<string, object>>()), Times.Never);
            _userRepoMock.Verify(r => r.AddAsync(It.IsAny<User>()), Times.Never);
        }

        [Theory]
        [InlineData("short7")]
        [InlineData("1234567")]
        public async Task Should_Throw_UserValidationException_When_Password_Is_Shorter_Than_Minimum(string shortPassword)
        {
            // Arrange
            var dto = new RegisterUserDto
            {
                Username = "validuser",
                Email = "valid@example.com",
                Password = shortPassword
            };

            // Act
            var exception = await Assert.ThrowsAsync<UserValidationException>(() => _service.CreateAsync(dto));

            // Assert
            Assert.Contains(nameof(dto.Password), exception.Errors.Keys);
            _userRepoMock.Verify(r => r.FindAsync(It.IsAny<Dictionary<string, object>>()), Times.Never);
            _userRepoMock.Verify(r => r.AddAsync(It.IsAny<User>()), Times.Never);
        }

        [Fact]
        public async Task Should_Throw_UserValidationException_When_Password_Exceeds_Maximum_Length()
        {
            // Arrange
            var dto = new RegisterUserDto
            {
                Username = "validuser",
                Email = "valid@example.com",
                Password = new string('p', 129)
            };

            // Act
            var exception = await Assert.ThrowsAsync<UserValidationException>(() => _service.CreateAsync(dto));

            // Assert
            Assert.Contains(nameof(dto.Password), exception.Errors.Keys);
            _userRepoMock.Verify(r => r.FindAsync(It.IsAny<Dictionary<string, object>>()), Times.Never);
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

            _userRepoMock.Setup(r => r.FindAsync(It.IsAny<Dictionary<string, object>>()))
                         .ReturnsAsync((User?)null);
            User? savedUser = null;
            _userRepoMock.Setup(r => r.AddAsync(It.IsAny<User>()))
                         .Callback<User>(user => savedUser = user)
                         .Returns(Task.CompletedTask);

            // Act
            await _service.CreateAsync(dto);

            // Assert
            Assert.NotNull(savedUser);
            Assert.NotEqual(dto.Password, savedUser.PasswordHash);

            var passwordHasher = new PasswordHasher<User>();
            Assert.Equal(
                PasswordVerificationResult.Success,
                passwordHasher.VerifyHashedPassword(savedUser, savedUser.PasswordHash, dto.Password));

            _userRepoMock.Verify(r => r.AddAsync(It.IsAny<User>()), Times.Once);
        }

        [Fact]
        public async Task Should_Preserve_Password_Whitespace_When_Hashing()
        {
            // Arrange
            const string password = " Pass word ";
            var dto = new RegisterUserDto
            {
                Username = "validuser",
                Email = "valid@example.com",
                Password = password
            };
            User? savedUser = null;
            _userRepoMock.Setup(r => r.FindAsync(It.IsAny<Dictionary<string, object>>()))
                         .ReturnsAsync((User?)null);
            _userRepoMock.Setup(r => r.AddAsync(It.IsAny<User>()))
                         .Callback<User>(user => savedUser = user)
                         .Returns(Task.CompletedTask);

            // Act
            await _service.CreateAsync(dto);

            // Assert
            Assert.NotNull(savedUser);
            Assert.Equal(
                PasswordVerificationResult.Success,
                new PasswordHasher<User>().VerifyHashedPassword(savedUser, savedUser.PasswordHash, password));
            Assert.Equal(
                PasswordVerificationResult.Failed,
                new PasswordHasher<User>().VerifyHashedPassword(savedUser, savedUser.PasswordHash, password.Trim()));
        }

        [Theory]
        [InlineData(null)]
        [InlineData("")]
        [InlineData("   ")]
        public void Should_Identify_Username_Parameter_When_User_Construction_Requires_It(string? username)
        {
            // Arrange
            var email = "valid@example.com";
            var passwordHash = "hash";

            // Act
            var exception = Assert.Throws<ArgumentException>(() =>
                new User(username!, email, passwordHash));

            // Assert
            Assert.Equal("username", exception.ParamName);
        }

        [Fact]
        public void Should_Identify_Username_Parameter_When_User_Construction_Exceeds_Maximum_Length()
        {
            // Arrange
            var username = new string('u', User.MaxUsernameLength + 1);

            // Act
            var exception = Assert.Throws<ArgumentOutOfRangeException>(() =>
                new User(username, "valid@example.com", "hash"));

            // Assert
            Assert.Equal("username", exception.ParamName);
        }

        [Theory]
        [InlineData(null)]
        [InlineData("")]
        [InlineData("   ")]
        public void Should_Identify_Email_Parameter_When_User_Construction_Requires_It(string? email)
        {
            // Arrange
            var username = "validuser";

            // Act
            var exception = Assert.Throws<ArgumentException>(() =>
                new User(username, email!, "hash"));

            // Assert
            Assert.Equal("email", exception.ParamName);
        }

        [Fact]
        public void Should_Identify_Email_Parameter_When_User_Construction_Exceeds_Maximum_Length()
        {
            // Arrange
            var email = new string('e', User.MaxEmailLength + 1);

            // Act
            var exception = Assert.Throws<ArgumentOutOfRangeException>(() =>
                new User("validuser", email, "hash"));

            // Assert
            Assert.Equal("email", exception.ParamName);
        }

        [Theory]
        [InlineData(null)]
        [InlineData("")]
        [InlineData("   ")]
        public void Should_Identify_PasswordHash_Parameter_When_User_Construction_Requires_It(string? passwordHash)
        {
            // Arrange
            var username = "validuser";
            var email = "valid@example.com";

            // Act
            var exception = Assert.Throws<ArgumentException>(() =>
                new User(username, email, passwordHash!));

            // Assert
            Assert.Equal("passwordHash", exception.ParamName);
        }

        [Fact]
        public void Should_Identify_PasswordHash_Parameter_When_User_Construction_Exceeds_Maximum_Length()
        {
            // Arrange
            var passwordHash = new string('h', 201);

            // Act
            var exception = Assert.Throws<ArgumentOutOfRangeException>(() =>
                new User("validuser", "valid@example.com", passwordHash));

            // Assert
            Assert.Equal("passwordHash", exception.ParamName);
        }

        [Fact]
        public void Should_Accept_User_Values_At_Maximum_Length()
        {
            // Arrange
            var username = new string('u', User.MaxUsernameLength);
            var email = new string('e', User.MaxEmailLength);
            var passwordHash = new string('h', 200);

            // Act
            var user = new User(username, email, passwordHash);

            // Assert
            Assert.Equal(User.MaxUsernameLength, user.Username.Length);
            Assert.Equal(User.MaxEmailLength, user.Email.Length);
            Assert.Equal(200, user.PasswordHash.Length);
        }

        [Theory]
        [InlineData(null)]
        [InlineData("")]
        [InlineData("   ")]
        public void Should_Identify_Username_Parameter_When_User_Update_Requires_It(string? username)
        {
            // Arrange
            var user = CreateUser();

            // Act
            var exception = Assert.Throws<ArgumentException>(() => user.UpdateUsername(username!));

            // Assert
            Assert.Equal("username", exception.ParamName);
        }

        [Fact]
        public void Should_Identify_Username_Parameter_When_User_Update_Exceeds_Maximum_Length()
        {
            // Arrange
            var user = CreateUser();
            var username = new string('u', User.MaxUsernameLength + 1);

            // Act
            var exception = Assert.Throws<ArgumentOutOfRangeException>(() => user.UpdateUsername(username));

            // Assert
            Assert.Equal("username", exception.ParamName);
        }

        [Theory]
        [InlineData(null)]
        [InlineData("")]
        [InlineData("   ")]
        public void Should_Identify_Email_Parameter_When_User_Update_Requires_It(string? email)
        {
            // Arrange
            var user = CreateUser();

            // Act
            var exception = Assert.Throws<ArgumentException>(() => user.UpdateEmail(email!));

            // Assert
            Assert.Equal("email", exception.ParamName);
        }

        [Fact]
        public void Should_Identify_Email_Parameter_When_User_Update_Exceeds_Maximum_Length()
        {
            // Arrange
            var user = CreateUser();
            var email = new string('e', User.MaxEmailLength + 1);

            // Act
            var exception = Assert.Throws<ArgumentOutOfRangeException>(() => user.UpdateEmail(email));

            // Assert
            Assert.Equal("email", exception.ParamName);
        }

        [Theory]
        [InlineData(null)]
        [InlineData("")]
        [InlineData("   ")]
        public void Should_Identify_PasswordHash_Parameter_When_User_Update_Requires_It(string? passwordHash)
        {
            // Arrange
            var user = CreateUser();

            // Act
            var exception = Assert.Throws<ArgumentException>(() => user.UpdatePasswordHash(passwordHash!));

            // Assert
            Assert.Equal("passwordHash", exception.ParamName);
        }

        [Fact]
        public void Should_Identify_PasswordHash_Parameter_When_User_Update_Exceeds_Maximum_Length()
        {
            // Arrange
            var user = CreateUser();
            var passwordHash = new string('h', 201);

            // Act
            var exception = Assert.Throws<ArgumentOutOfRangeException>(() => user.UpdatePasswordHash(passwordHash));

            // Assert
            Assert.Equal("passwordHash", exception.ParamName);
        }

        private static User CreateUser() => new("validuser", "valid@example.com", "hash");
    }
}
