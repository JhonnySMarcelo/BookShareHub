using BookShareHub.Application.Books.Services;
using BookShareHub.Domain.Books.Entities;
using BookShareHub.Domain.Books.Repositories;
using Moq;

namespace BookShareHub.Tests.Application.Books.BookServiceTests
{
    public class BookService_GetByIdAsync_Tests
    {
        private readonly Mock<IBookRepository> _bookRepoMock;
        private readonly BookService _service;

        public BookService_GetByIdAsync_Tests()
        {
            _bookRepoMock = new Mock<IBookRepository>();
            _service = new BookService(_bookRepoMock.Object);
        }

        [Fact]
        public async Task Should_Return_Book_When_Id_Is_Valid()
        {
            // Arrange
            var book = new Book(
                title: "Clean Architecture",
                author: "Robert Martin",
                ownerId: Guid.NewGuid(),
                available: true,
                description: "Building upon the success of..."
            );

            _bookRepoMock.Setup(r => r.GetByIdAsync(book.Id))
                         .ReturnsAsync(book);

            // Act
            var result = await _service.GetByIdAsync(book.Id);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(book.Title, result.Title);
            Assert.Equal(book.Author, result.Author);
            _bookRepoMock.Verify(r => r.GetByIdAsync(book.Id), Times.Once);
        }
    }
}
