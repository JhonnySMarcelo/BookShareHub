namespace BookShareHub.Application.Users.DTOs
{
    public record LoginUserDto
    {
        public string Email { get; set; } = "";
        public string Password { get; set; } = "";
    }
}
