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

        public async Task<Book?> GetByIdAsync(Guid id)
        {
            using var connection = new SqlConnection(_connectionString);
            await connection.OpenAsync();

            var command = new SqlCommand(
                "SELECT Id, Title, Author, Description, Available, CreationTime, OwnerId, BorrowerId " +
                "FROM Books WHERE Id = @Id", connection);

            command.Parameters.AddWithValue("@Id", id);

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
    }
}
