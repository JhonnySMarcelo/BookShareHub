using BookShareHub.Application.Configurations;
using BookShareHub.Application.Users.DTOs;
using BookShareHub.Domain.Users.Entities;
using BookShareHub.Domain.Users.Repositories;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
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

        public async Task<User> CreateAsync(RegisterUserDto dto)
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

            var passwordHasher = new PasswordHasher<User>();
            var passwordHash = passwordHasher.HashPassword(user, dto.Password);

            user.UpdatePasswordHash(passwordHash);

            await _userRepository.AddAsync(user);

            return user;
        }
        public async Task<User?> GetByIdAsync(Guid id)
        {
            if (id == Guid.Empty)
                throw new ArgumentException("Id cannot be empty.", nameof(id));

            return await _userRepository.FindAsync(new Dictionary<string, object>
            {
                { "Id", id }
            });
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

        public async Task<User?> PatchAsync(Guid id, UpdateUserDto dto)
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
                user.UpdatePasswordHash(HashPassword(dto.Password));

            return await _userRepository.PatchAsync(user);
        }

        private string HashPassword(string password)
        {
            using var sha = SHA256.Create();
            var bytes = sha.ComputeHash(Encoding.UTF8.GetBytes(password));
            return Convert.ToBase64String(bytes);
        }

    }
}
