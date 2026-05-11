namespace BookShareHub.Domain.Books.Entities
{
    /// <summary>
    /// Represents a book in the BookShareHub system.
    /// </summary>
    public class Book
    {
        /// <summary>
        /// The unique identifier of the book.
        /// </summary>
        public Guid Id { get; init; } = Guid.NewGuid();

        /// <summary>
        /// The title of the book.
        /// </summary>
        public string Title { get; private set; } = null!;

        /// <summary>
        /// The author of the book.
        /// </summary>
        public string Author { get; private set; } = null!;

        /// <summary>
        /// A brief description of the book.
        /// </summary>
        public string? Description { get; private set; } = null;

        /// <summary>
        /// Indicates whether the book is available for borrowing.
        /// </summary>
        public bool Available { get; private set; } = true;

        /// <summary>
        /// The date and time when the book was created.
        /// </summary>
        public DateTime CreationTime { get; init; } = DateTime.UtcNow;

        /// <summary>
        /// The unique identifier of the book's owner.
        /// </summary>
        public Guid OwnerId { get; private set; }

        /// <summary>
        /// The unique identifier of the borrower, if the book is currently borrowed.
        /// </summary>
        public Guid? BorrowerId { get; private set; }

        protected Book() { }

        public Book(string title, string author, string? description, bool available, Guid ownerId)
        {
            if (string.IsNullOrWhiteSpace(title))
                throw new ArgumentException("Book Title is required.");

            if (string.IsNullOrWhiteSpace(author))
                throw new ArgumentException("Book Author is required.");

            if (ownerId == Guid.Empty)
                throw new ArgumentException("Book OwnerId is required.");

            Title = title;
            Author = author;
            Description = description;
            Available = available;
            OwnerId = ownerId;
    }
}
