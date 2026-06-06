namespace BookShareHub.Application.Books.DTOs
{
    public record GetBookDto
    {
        public Guid Id { get; set; }
        public string Title { get; set; } = string.Empty;
        public string Author { get; set; } = string.Empty;
        public string? Description { get; set; }
        public bool Available { get; set; }
        public bool IsOwner { get; set; }
    }
}
