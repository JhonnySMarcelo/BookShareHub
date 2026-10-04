namespace BookShareHub.Application.Books.DTOs.Requests
{
    public record CreateBookRequest
    {
        public string Title { get; set; } = string.Empty;
        public string Author { get; set; } = string.Empty;
        public string? Description { get; set; }
        public bool Available { get; init; }
    }
}
