namespace BookShareHub.Application.Books.DTOs.Requests
{
    public sealed record CreateBookRequest
    {
        public string Title { get; init; } = string.Empty;
        public string Author { get; init; } = string.Empty;
        public string? Description { get; init; }
        public bool Available { get; init; }
    }
}
