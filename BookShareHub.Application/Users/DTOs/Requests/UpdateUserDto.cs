namespace BookShareHub.Application.Users.DTOs.Requests
{
    public sealed record UpdateUserDto
    {
        public string? Username { get; init; }
        public string? Email { get; init; }
        public string? Password { get; init; }
    }
}
