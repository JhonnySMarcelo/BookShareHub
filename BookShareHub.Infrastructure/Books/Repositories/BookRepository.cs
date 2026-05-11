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
    }
}
