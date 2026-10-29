namespace BookShareHub.Application.Users.DTOs.Responses
{
    public sealed record LogoutResponse
    {
        public string Message { get; init; } = string.Empty;
    }
}
