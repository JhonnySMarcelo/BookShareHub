namespace BookShareHub.Application.Users.DTOs.Requests
{
    public sealed record LoginUserDto
    {
        public string Email { get; init; } = "";
        public string Password { get; init; } = "";
    }
}
