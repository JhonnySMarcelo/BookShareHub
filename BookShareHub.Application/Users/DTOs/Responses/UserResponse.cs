namespace BookShareHub.Application.Users.DTOs.Responses
{
    public sealed record UserResponse
    {
        public Guid Id { get; init; }
        public string Username { get; init; } = string.Empty;
        public string Email { get; init; } = string.Empty;
        public DateTime CreationTime { get; init; }
    }
}