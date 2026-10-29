using BookShareHub.Application.Configurations;
using BookShareHub.Application.Users.DTOs.Requests;
using BookShareHub.Application.Users.DTOs.Responses;
using BookShareHub.Application.Users.Mappers;
using BookShareHub.Domain.Users.Entities;
using BookShareHub.Domain.Users.Repositories;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;
using System.Security.Cryptography;
using System.Text;

namespace BookShareHub.Application.Users.Services
{
    public class UserService
    {
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

            var existing = await _userRepository.FindAsync(new Dictionary<string, object>
            {
                { "Email", dto.Email },
                { "Username", dto.Username }
            });

            if (existing != null)
                throw new InvalidOperationException("User with same email or username already exists.");

            var user = new User(dto.Username, dto.Email, "TempHash");

            user.UpdatePasswordHash(HashPassword(user, dto.Password));

            await _userRepository.AddAsync(user);

            return UserMapper.ToResponse(user);
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
                user.UpdateUsername(dto.Username);

            if (dto.Email != null)
                user.UpdateEmail(dto.Email);

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
