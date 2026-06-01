using BookShareHub.Domain.Books.Entities;
using BookShareHub.Domain.Books.Repositories;
using Microsoft.Data.SqlClient;

namespace BookShareHub.Infrastructure.Books.Repositories
{
    public class BookRepository : IBookRepository
    {
        private readonly string _connectionString;

        public BookRepository(string connectionString)
        {
            _connectionString = connectionString ?? throw new ArgumentNullException(nameof(connectionString));
        }

        public async Task AddAsync(Book book)
        {
            using var connection = new SqlConnection(_connectionString);
            await connection.OpenAsync();

            var command = new SqlCommand(
                "INSERT INTO Books (Id, Title, Author, Description, Available, CreationTime, OwnerId, BorrowerId) " +
                "VALUES (@Id, @Title, @Author, @Description, @Available, @CreationTime, @OwnerId, @BorrowerId)", connection);

            command.Parameters.AddWithValue("@Id", book.Id);
            command.Parameters.AddWithValue("@Title", book.Title);
            command.Parameters.AddWithValue("@Author", book.Author);
            command.Parameters.AddWithValue("@Description", book.Description == null ? DBNull.Value : book.Description);
            command.Parameters.AddWithValue("@Available", book.Available);
            command.Parameters.AddWithValue("@CreationTime", book.CreationTime);
            command.Parameters.AddWithValue("@OwnerId", book.OwnerId);
            command.Parameters.AddWithValue("@BorrowerId", DBNull.Value);


            await command.ExecuteNonQueryAsync();
        }

        public async Task<Book?> GetByIdForOwnerAsync(Guid id, Guid userId)
        {
            using var connection = new SqlConnection(_connectionString);
            await connection.OpenAsync();

            var command = new SqlCommand(
                "SELECT Id, Title, Author, Description, Available, CreationTime, OwnerId, BorrowerId " +
                "FROM Books WHERE Id = @Id AND OwnerId = @OwnerId", connection);

            command.Parameters.AddWithValue("@Id", id);
            command.Parameters.AddWithValue("@OwnerId", userId);

            using var reader = await command.ExecuteReaderAsync();

            if (await reader.ReadAsync())
            {
                var book = new Book(
                    title: reader.GetString(reader.GetOrdinal("Title")),
                    author: reader.GetString(reader.GetOrdinal("Author")),
                    description: reader.IsDBNull(reader.GetOrdinal("Description")) ? null : reader.GetString(reader.GetOrdinal("Description")),
                    available: reader.GetBoolean(reader.GetOrdinal("Available")),
                    ownerId: reader.GetGuid(reader.GetOrdinal("OwnerId"))
                );

                typeof(Book).GetProperty(nameof(Book.Id))?.SetValue(book, reader.GetGuid(reader.GetOrdinal("Id")));
                typeof(Book).GetProperty(nameof(Book.CreationTime))?.SetValue(book, reader.GetDateTime(reader.GetOrdinal("CreationTime")));

                var borrowerIdOrdinal = reader.GetOrdinal("BorrowerId");
                Guid? borrowerId = reader.IsDBNull(borrowerIdOrdinal) ? null : reader.GetGuid(borrowerIdOrdinal);
                typeof(Book).GetProperty(nameof(Book.BorrowerId))?.SetValue(book, borrowerId);

                return book;
            }

            return null;
        }

        public async Task<List<Book>> GetAllAsync()
        {
            var books = new List<Book>();

            using var connection = new SqlConnection(_connectionString);
            await connection.OpenAsync();

            var command = new SqlCommand(
                "SELECT Id, Title, Author, Description, Available, CreationTime, OwnerId, BorrowerId FROM Books",
                connection
            );

            using var reader = await command.ExecuteReaderAsync();

            while (await reader.ReadAsync())
            {
                var book = new Book(
                    title: reader.GetString(reader.GetOrdinal("Title")),
                    author: reader.GetString(reader.GetOrdinal("Author")),
                    description: reader.IsDBNull(reader.GetOrdinal("Description")) ? null : reader.GetString(reader.GetOrdinal("Description")),
                    available: reader.GetBoolean(reader.GetOrdinal("Available")),
                    ownerId: reader.GetGuid(reader.GetOrdinal("OwnerId"))
                );

                typeof(Book).GetProperty(nameof(Book.Id))?.SetValue(book, reader.GetGuid(reader.GetOrdinal("Id")));
                typeof(Book).GetProperty(nameof(Book.CreationTime))?.SetValue(book, reader.GetDateTime(reader.GetOrdinal("CreationTime")));

                var borrowerIdOrdinal = reader.GetOrdinal("BorrowerId");
                Guid? borrowerId = reader.IsDBNull(borrowerIdOrdinal) ? null : reader.GetGuid(borrowerIdOrdinal);
                typeof(Book).GetProperty(nameof(Book.BorrowerId))?.SetValue(book, borrowerId);

                books.Add(book);
            }

            return books;
        }

        public async Task<Book?> PatchAsync(Book book, Guid userId)
        {
            using var connection = new SqlConnection(_connectionString);
            await connection.OpenAsync();

            var updates = new List<string>();
            var command = new SqlCommand();
            command.Connection = connection;

            if (book.Title != null)
            {
                updates.Add("Title = @Title");
                command.Parameters.AddWithValue("@Title", book.Title);
            }
            if (book.Author != null)
            {
                updates.Add("Author = @Author");
                command.Parameters.AddWithValue("@Author", book.Author);
            }
            if (book.Description != null)
            {
                updates.Add("Description = @Description");
                command.Parameters.AddWithValue("@Description", book.Description);
            }

            updates.Add("Available = @Available");
            command.Parameters.AddWithValue("@Available", book.Available);

            updates.Add("OwnerId = @OwnerId");
            command.Parameters.AddWithValue("@OwnerId", book.OwnerId);

            updates.Add("BorrowerId = @BorrowerId");
            command.Parameters.AddWithValue("@BorrowerId", book.BorrowerId == null ? DBNull.Value : book.BorrowerId);

            if (!updates.Any())
                return await GetByIdForOwnerAsync(book.Id, userId);

            command.CommandText = $"UPDATE Books SET {string.Join(", ", updates)} WHERE Id = @Id AND OwnerId = @OwnerId";
            command.Parameters.AddWithValue("@Id", book.Id);
            command.Parameters.AddWithValue("@OwnerId", userId);

            var rowsAffected = await command.ExecuteNonQueryAsync();
            if (rowsAffected == 0) return null;

            return await GetByIdForOwnerAsync(book.Id, userId);
        }

        public async Task<bool> DeleteAsync(Guid id, Guid userId)
        {
            using var connection = new SqlConnection(_connectionString);
            await connection.OpenAsync();

            var command = new SqlCommand("DELETE FROM Books WHERE Id = @Id AND OwnerId = @OwnerId", connection);
            command.Parameters.AddWithValue("@Id", id);
            command.Parameters.AddWithValue("@OwnerId", userId);

            var rowsAffected = await command.ExecuteNonQueryAsync();
            return rowsAffected > 0;
        }
    }
}
