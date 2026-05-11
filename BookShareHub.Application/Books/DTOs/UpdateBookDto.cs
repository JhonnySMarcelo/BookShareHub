namespace BookShareHub.Application.Books.DTOs
{
    public record UpdateBookDto
    {
        public string? Title { get; init; }
        public string? Author { get; init; }
        public string? Description { get; init; }
        public bool? Available { get; init; }
    }
}
