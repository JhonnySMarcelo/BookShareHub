namespace BookShareHub.Application.Users.Exceptions
{
    public sealed class UserAlreadyExistsException : Exception
    {
        public UserAlreadyExistsException()
            : base("Username or email is already in use.")
        {
        }
    }
}
