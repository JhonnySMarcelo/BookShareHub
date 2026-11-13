namespace BookShareHub.Application.Books.Exceptions
{
    /// <summary>
    /// Indicates that one or more book fields failed validation.
    /// </summary>
    public sealed class BookValidationException : Exception
    {
        public IDictionary<string, string[]> Errors { get; }

        public BookValidationException(IDictionary<string, string[]> errors)
            : base("One or more book fields are invalid.")
        {
            Errors = errors ?? throw new ArgumentNullException(nameof(errors));
        }
    }
}
