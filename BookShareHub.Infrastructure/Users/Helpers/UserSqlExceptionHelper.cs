namespace BookShareHub.Infrastructure.Users.Helpers
{
    internal static class UserSqlExceptionHelper
    {
        private const int UniqueIndexViolation = 2601;
        private const int UniqueConstraintViolation = 2627;

        public static bool IsUsernameOrEmailUniqueConstraintViolation(int errorNumber, string message)
        {
            if (errorNumber is not (UniqueIndexViolation or UniqueConstraintViolation))
                return false;

            return message.Contains("'UQ_Users_Username'", StringComparison.OrdinalIgnoreCase)
                || message.Contains("'UQ_Users_Email'", StringComparison.OrdinalIgnoreCase);
        }
    }
}
