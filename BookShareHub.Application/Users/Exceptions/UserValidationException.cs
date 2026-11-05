namespace BookShareHub.Application.Users.Exceptions
{
    public sealed class UserValidationException : Exception
    {
        public IDictionary<string, string[]> Errors { get; }

        public UserValidationException(IDictionary<string, string[]> errors)
            : base("One or more user fields are invalid.")
        {
            Errors = errors ?? throw new ArgumentNullException(nameof(errors));
        }
    }
}
