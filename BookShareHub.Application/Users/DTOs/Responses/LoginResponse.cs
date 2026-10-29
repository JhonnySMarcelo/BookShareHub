namespace BookShareHub.Application.Users.DTOs.Responses
{
    public sealed record LoginResponse
    {
        public string Token { get; init; } = string.Empty;
    }
}
