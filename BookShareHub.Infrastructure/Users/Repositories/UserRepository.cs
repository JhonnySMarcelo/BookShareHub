using BookShareHub.Domain.Users.Entities;
using BookShareHub.Domain.Users.Repositories;
using Microsoft.Data.SqlClient;

namespace BookShareHub.Infrastructure.Users.Repositories
{
    public class UserRepository : IUserRepository
    {
        private readonly string _connectionString;

        public UserRepository(string connectionString)
        {
            _connectionString = connectionString ?? throw new ArgumentNullException(nameof(connectionString));
        }

        public async Task AddAsync(User user)
        {
            using var connection = new SqlConnection(_connectionString);
            await connection.OpenAsync();

            var command = new SqlCommand(
                "INSERT INTO Users (Id, Username, PasswordHash, Email, CreationTime) " +
                "VALUES (@Id, @Username, @PasswordHash, @Email, @CreationTime)", connection);

            command.Parameters.AddWithValue("@Id", user.Id);
            command.Parameters.AddWithValue("@Username", user.Username);
            command.Parameters.AddWithValue("@PasswordHash", user.PasswordHash);
            command.Parameters.AddWithValue("@Email", user.Email);
            command.Parameters.AddWithValue("@CreationTime", user.CreationTime);

            await command.ExecuteNonQueryAsync();
        }

        public async Task<User?> FindAsync(Dictionary<string, object> filters)
        {
            using var connection = new SqlConnection(_connectionString);
            await connection.OpenAsync();

            var whereClauses = new List<string>();
            var command = connection.CreateCommand();

            foreach (var filter in filters)
            {
                var paramName = $"@{filter.Key}";
                whereClauses.Add($"{filter.Key} = {paramName}");
                command.Parameters.AddWithValue(paramName, filter.Value);
            }

            var whereSql = string.Join(" OR ", whereClauses);
            command.CommandText = @"SELECT TOP 1 Id, Username, Email, PasswordHash, CreationTime 
                            FROM Users 
                            WHERE " + whereSql;

            using var reader = await command.ExecuteReaderAsync();
            if (await reader.ReadAsync())
            {
                var user = new User(
                    reader["Username"].ToString()!,
                    reader["Email"].ToString()!,
                    reader["PasswordHash"].ToString()!
                )
                {
                    Id = Guid.Parse(reader["Id"].ToString()!),
                    CreationTime = Convert.ToDateTime(reader["CreationTime"])
                };

                return user;
            }

            return null;
        }


        //public async Task<User?> GetByIdAsync(Guid id)
        //{
        //    using var connection = new SqlConnection(_connectionString);
        //    await connection.OpenAsync();

        //    var command = new SqlCommand(
        //        "SELECT Id, Username, PasswordHash, Email, CreationTime FROM Users WHERE Id = @Id", connection);

        //    command.Parameters.AddWithValue("@Id", id);

        //    using var reader = await command.ExecuteReaderAsync();

        //    if (await reader.ReadAsync())
        //    {
        //        var user = new User(
        //            username: reader.GetString(reader.GetOrdinal("Username")),
        //            email: reader.GetString(reader.GetOrdinal("Email")),
        //            passwordHash: reader.GetString(reader.GetOrdinal("PasswordHash"))
        //        );

        //        typeof(User).GetProperty(nameof(User.Id))?.SetValue(user, reader.GetGuid(reader.GetOrdinal("Id")));
        //        typeof(User).GetProperty(nameof(User.CreationTime))?.SetValue(user, reader.GetDateTime(reader.GetOrdinal("CreationTime")));

        //        return user;
        //    }

        //    return null;
        //}

        //public async Task<User?> GetByEmailAsync(string email)
        //{
        //    using var connection = new SqlConnection(_connectionString);
        //    await connection.OpenAsync();

        //    var command = new SqlCommand(
        //        "SELECT Id, Username, PasswordHash, Email, CreationTime FROM Users WHERE Email = @Email", connection);

        //    command.Parameters.AddWithValue("@Email", email);

        //    using var reader = await command.ExecuteReaderAsync();

        //    if (await reader.ReadAsync())
        //    {
        //        var user = new User(
        //            username: reader.GetString(reader.GetOrdinal("Username")),
        //            email: reader.GetString(reader.GetOrdinal("Email")),
        //            passwordHash: reader.GetString(reader.GetOrdinal("PasswordHash"))
        //        );

        //        typeof(User).GetProperty(nameof(User.Id))?.SetValue(user, reader.GetGuid(reader.GetOrdinal("Id")));
        //        typeof(User).GetProperty(nameof(User.CreationTime))?.SetValue(user, reader.GetDateTime(reader.GetOrdinal("CreationTime")));

        //        return user;
        //    }

        //    return null;
        //}

        //public async Task<List<User>> GetAllAsync()
        //{
        //    var users = new List<User>();

        //    using var connection = new SqlConnection(_connectionString);
        //    await connection.OpenAsync();

        //    var command = new SqlCommand("SELECT Id, Username, PasswordHash, Email, CreationTime FROM Users", connection);

        //    using var reader = await command.ExecuteReaderAsync();

        //    while (await reader.ReadAsync())
        //    {
        //        var user = new User(
        //            username: reader.GetString(reader.GetOrdinal("Username")),
        //            email: reader.GetString(reader.GetOrdinal("Email")),
        //            passwordHash: reader.GetString(reader.GetOrdinal("PasswordHash"))
        //        );

        //        typeof(User).GetProperty(nameof(User.Id))?.SetValue(user, reader.GetGuid(reader.GetOrdinal("Id")));
        //        typeof(User).GetProperty(nameof(User.CreationTime))?.SetValue(user, reader.GetDateTime(reader.GetOrdinal("CreationTime")));

        //        users.Add(user);
        //    }

        //    return users;
        //}

        public async Task<User?> PatchAsync(User user)
        {
            using var connection = new SqlConnection(_connectionString);
            await connection.OpenAsync();

            var updates = new List<string>();
            var command = new SqlCommand();
            command.Connection = connection;

            if (user.Username != null)
            {
                updates.Add("Username = @Username");
                command.Parameters.AddWithValue("@Username", user.Username);
            }
            if (user.Email != null)
            {
                updates.Add("Email = @Email");
                command.Parameters.AddWithValue("@Email", user.Email);
            }
            if (user.PasswordHash != null)
            {
                updates.Add("PasswordHash = @PasswordHash");
                command.Parameters.AddWithValue("@PasswordHash", user.PasswordHash);
            }

            if (!updates.Any())
                return await FindAsync(new Dictionary<string, object>
                {
                    { "Id", user.Id }
                });

            command.CommandText = $"UPDATE Users SET {string.Join(", ", updates)} WHERE Id = @Id";
            command.Parameters.AddWithValue("@Id", user.Id);

            var rowsAffected = await command.ExecuteNonQueryAsync();
            if (rowsAffected == 0) return null;

            return await FindAsync(new Dictionary<string, object>
            {
                { "Id", user.Id }
            });
        }

        public async Task<bool> DeleteAsync(Guid id)
        {
            using var connection = new SqlConnection(_connectionString);
            await connection.OpenAsync();

            var command = new SqlCommand("DELETE FROM Users WHERE Id = @Id", connection);
            command.Parameters.AddWithValue("@Id", id);

            var rowsAffected = await command.ExecuteNonQueryAsync();
            return rowsAffected > 0;
        }
    }
}
