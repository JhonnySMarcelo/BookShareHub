using BookShareHub.Application.Configurations;
using BookShareHub.Application.Users.DTOs.Requests;
using BookShareHub.Application.Users.DTOs.Responses;
using BookShareHub.Application.Users.Exceptions;
using BookShareHub.Application.Users.Mappers;
using BookShareHub.Domain.Users.Entities;
using BookShareHub.Domain.Users.Repositories;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;
using System.Net.Mail;

namespace BookShareHub.Application.Users.Services
{
    public class UserService
    {
        private const int MinPasswordLength = 8;
        private const int MaxPasswordLength = 128;

        private readonly IUserRepository _userRepository;
        private readonly JwtSettings _jwtSettings;

        public UserService(IUserRepository userRepository, IOptions<JwtSettings> jwtSettings)
        {
            _userRepository = userRepository ?? throw new ArgumentNullException(nameof(userRepository));
            _jwtSettings = jwtSettings.Value ?? throw new ArgumentNullException(nameof(jwtSettings));
        }

        public async Task<UserResponse> CreateAsync(RegisterUserDto dto)
        {
            if (dto == null)
                throw new ArgumentNullException(nameof(dto));

            var (username, email) = NormalizeAndValidateRegistration(dto);

            User user;
            try
            {
                user = new User(username, email, "TempHash");
            }
            catch (ArgumentException exception) when (exception.ParamName is "username" or "email")
            {
                var field = exception.ParamName == "username"
                    ? nameof(RegisterUserDto.Username)
                    : nameof(RegisterUserDto.Email);
                throw new UserValidationException(new Dictionary<string, string[]>
                {
                    [field] = [exception.Message]
                });
            }

            var existing = await _userRepository.FindAsync(new Dictionary<string, object>
            {
                { "Email", email },
                { "Username", username }
            });

            if (existing != null)
                throw new UserAlreadyExistsException();

            user.UpdatePasswordHash(HashPassword(user, dto.Password));

            await _userRepository.AddAsync(user);

            return UserMapper.ToResponse(user);
        }

        private static (string Username, string Email) NormalizeAndValidateRegistration(RegisterUserDto dto)
        {
            var errors = new Dictionary<string, string[]>();

            if (string.IsNullOrWhiteSpace(dto.Password))
                errors[nameof(dto.Password)] = ["Password is required."];
            else if (dto.Password.Length < MinPasswordLength || dto.Password.Length > MaxPasswordLength)
                errors[nameof(dto.Password)] = [$"Password must be between {MinPasswordLength} and {MaxPasswordLength} characters."];

            var username = dto.Username?.Trim() ?? string.Empty;
            var email = dto.Email?.Trim() ?? string.Empty;

            if (email.Length > 0 && !IsEmailAddress(email))
                errors[nameof(dto.Email)] = ["Email address is not valid."];

            if (errors.Count > 0)
                throw new UserValidationException(errors);

            return (username, email);
        }

        private static bool IsEmailAddress(string email)
        {
            return MailAddress.TryCreate(email, out var address)
                && string.Equals(address.Address, email, StringComparison.OrdinalIgnoreCase);
        }

        public async Task<UserResponse?> GetByIdAsync(Guid id)
        {
            if (id == Guid.Empty)
                throw new ArgumentException("Id cannot be empty.", nameof(id));

            var user = await _userRepository.FindAsync(new Dictionary<string, object>
            {
                { "Id", id }
            });

            return user is null ? null : UserMapper.ToResponse(user);
        }

        public async Task<bool?> DeleteAsync(Guid id)
        {
            if (id == Guid.Empty)
                throw new ArgumentException("User Id cannot be empty.", nameof(id));

            var user = await _userRepository.FindAsync(new Dictionary<string, object>
            {
                { "Id", id }
            }); 

            if (user == null) return null;

            await _userRepository.DeleteAsync(id);

            return true;
        }

        public async Task<UserResponse?> PatchAsync(Guid id, UpdateUserDto dto)
        {
            if (id == Guid.Empty)
                throw new ArgumentException("User Id cannot be empty.", nameof(id));

            if (dto == null)
                throw new ArgumentNullException(nameof(dto));

            var user = await _userRepository.FindAsync(new Dictionary<string, object>
            {
                { "Id", id }
            });

            if (user == null)
                return null;

            if (dto.Username != null)
                user.UpdateUsername(dto.Username.Trim());

            if (dto.Email != null)
                user.UpdateEmail(dto.Email.Trim());

            if (dto.Password != null)
                user.UpdatePasswordHash(HashPassword(user, dto.Password));

            var updatedUser = await _userRepository.PatchAsync(user);

            return updatedUser is null ? null : UserMapper.ToResponse(updatedUser);
        }

        private string HashPassword(User user, string password)
        {
            var passwordHasher = new PasswordHasher<User>();
            return passwordHasher.HashPassword(user, password);
        }

    }
}
