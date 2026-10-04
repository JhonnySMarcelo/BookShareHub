namespace BookShareHub.Application.Books.DTOs.Responses
{
    public record BookResponse
    {
        public Guid Id { get; init; }
        public string Title { get; init; } = string.Empty;
        public string Author { get; init; } = string.Empty;
        public string? Description { get; init; }
        public bool Available { get; init; }
        public bool IsOwner { get; init; }
    }
}
