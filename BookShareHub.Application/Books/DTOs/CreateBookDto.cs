namespace BookShareHub.Application.Books.DTOs
{
    public record CreateBookDto
    {
        public string Title { get; set; } = string.Empty;
        public string Author { get; set; } = string.Empty;
        public string? Description { get; set; }
        public bool Available { get; set; } = true;
        public Guid OwnerId { get; set; }
    }
}
