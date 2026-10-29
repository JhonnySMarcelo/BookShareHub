using BookShareHub.Application.Users.DTOs.Responses;
using BookShareHub.Domain.Users.Entities;

namespace BookShareHub.Application.Users.Mappers
{
    public static class UserMapper
    {
        public static UserResponse ToResponse(User user)
        {
            return new UserResponse
            {
                Id = user.Id,
                Username = user.Username,
                Email = user.Email,
                CreationTime = user.CreationTime
            };
        }
    }
}
