namespace BookShareHub.Domain.Users.Entities
{
    /// <summary>
    /// Represents a user in the BookShareHub system.
    /// </summary>
    public class User
    {
        private const int MaxUsernameLength = 100;
        private const int MaxEmailLength = 200;
        private const int MaxPasswordHashLength = 200;

        /// <summary>
        /// The unique identifier of the user.
        /// </summary>
        public Guid Id { get; init; } = Guid.NewGuid();

        /// <summary>
        /// The username chosen by the user.
        /// </summary>
        public string Username { get; private set; } = null!;

        /// <summary>
        /// The email address of the user.
        /// </summary>
        public string Email { get; private set; } = null!;

        /// <summary>
        /// The hashed password of the user.
        /// </summary>
        public string PasswordHash { get; private set; } = null!;

        /// <summary>
        /// The date and time when the user was created.
        /// </summary>
        public DateTime CreationTime { get; init; } = DateTime.UtcNow;

        protected User() { }

        public User(string username, string email, string passwordHash)
        {
            if (string.IsNullOrWhiteSpace(username))
                throw new ArgumentException("Username is required.");

            if (username.Length > MaxUsernameLength)
                throw new ArgumentOutOfRangeException(nameof(username), $"Username cannot exceed {MaxUsernameLength} characters.");

            if (string.IsNullOrWhiteSpace(email))
                throw new ArgumentException("Email is required.");

            if (email.Length > MaxEmailLength)
                throw new ArgumentOutOfRangeException(nameof(email), $"Email cannot exceed {MaxEmailLength} characters.");

            if (string.IsNullOrWhiteSpace(passwordHash))
                throw new ArgumentException("PasswordHash is required.");

            if (passwordHash.Length > MaxPasswordHashLength)
                throw new ArgumentOutOfRangeException(nameof(passwordHash), $"PasswordHash cannot exceed {MaxPasswordHashLength} characters.");

            Username = username;
            Email = email;
            PasswordHash = passwordHash;
        }

        public void UpdateUsername(string username)
        {
            if (string.IsNullOrWhiteSpace(username))
                throw new ArgumentException("Username is required.");

            if (username.Length > MaxUsernameLength)
                throw new ArgumentOutOfRangeException(nameof(username), $"Username cannot exceed {MaxUsernameLength} characters.");

            Username = username;
        }

        public void UpdateEmail(string email)
        {
            if (string.IsNullOrWhiteSpace(email))
                throw new ArgumentException("Email is required.");

            if (email.Length > MaxEmailLength)
                throw new ArgumentOutOfRangeException(nameof(email), $"Email cannot exceed {MaxEmailLength} characters.");

            Email = email;
        }

        public void UpdatePasswordHash(string passwordHash)
        {
            if (string.IsNullOrWhiteSpace(passwordHash))
                throw new ArgumentException("PasswordHash is required.");

            if (passwordHash.Length > MaxPasswordHashLength)
                throw new ArgumentOutOfRangeException(nameof(passwordHash), $"PasswordHash cannot exceed {MaxPasswordHashLength} characters.");

            PasswordHash = passwordHash;
        }
    }
}
