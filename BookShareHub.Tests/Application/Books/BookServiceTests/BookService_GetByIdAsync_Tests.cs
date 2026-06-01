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
        private readonly Guid _ownerId;

        public BookService_GetByIdAsync_Tests()
        {
            _bookRepoMock = new Mock<IBookRepository>();
            _service = new BookService(_bookRepoMock.Object);
            _ownerId = Guid.NewGuid();
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

            _bookRepoMock.Setup(r => r.GetByIdForOwnerAsync(book.Id, _ownerId))
                         .ReturnsAsync(book);

            // Act
            var result = await _service.GetByIdAsync(book.Id, _ownerId);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(book.Title, result.Title);
            Assert.Equal(book.Author, result.Author);
            _bookRepoMock.Verify(r => r.GetByIdForOwnerAsync(book.Id, _ownerId), Times.Once);
        }

        [Fact]
        public async Task Should_Throw_ArgumentException_When_Id_Is_Empty()
        {
            // Arrange
            var emptyId = Guid.Empty;

            // Act & Assert
            await Assert.ThrowsAsync<ArgumentException>(() => _service.GetByIdAsync(emptyId, _ownerId));

            // Assert
            _bookRepoMock.Verify(r => r.GetByIdForOwnerAsync(It.IsAny<Guid>(), _ownerId), Times.Never);
        }

        [Fact]
        public async Task Should_Return_Null_When_Book_Not_Found()
        {
            // Arrange
            var id = Guid.NewGuid();
            _bookRepoMock.Setup(r => r.GetByIdForOwnerAsync(id, _ownerId))
                         .ReturnsAsync((Book?)null);

            // Act
            var result = await _service.GetByIdAsync(id, _ownerId);

            // Assert
            Assert.Null(result);
            _bookRepoMock.Verify(r => r.GetByIdForOwnerAsync(id, _ownerId), Times.Once);
        }

        [Fact]
        public async Task Should_Return_Book_With_BorrowerId_When_Present()
        {
            // Arrange
            var borrowerId = Guid.NewGuid();
            var book = new Book(
                title: "Domain-Driven Design",
                author: "Eric Evans",
                ownerId: Guid.NewGuid(),
                available: false,
                description: "The blue book"
            );
            typeof(Book).GetProperty(nameof(Book.BorrowerId))?.SetValue(book, borrowerId);

            _bookRepoMock.Setup(r => r.GetByIdForOwnerAsync(book.Id, _ownerId))
                         .ReturnsAsync(book);

            // Act
            var result = await _service.GetByIdAsync(book.Id, _ownerId);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(borrowerId, result.BorrowerId);
            _bookRepoMock.Verify(r => r.GetByIdForOwnerAsync(book.Id, _ownerId), Times.Once);
        }

        [Fact]
        public async Task Should_Return_Book_With_Null_BorrowerId_When_Not_Present()
        {
            // Arrange
            var book = new Book(
                title: "Refactoring",
                author: "Martin Fowler",
                ownerId: Guid.NewGuid(),
                available: true,
                description: "Improving the design of existing code"
            );
            typeof(Book).GetProperty(nameof(Book.BorrowerId))?.SetValue(book, null);

            _bookRepoMock.Setup(r => r.GetByIdForOwnerAsync(book.Id, _ownerId))
                         .ReturnsAsync(book);

            // Act
            var result = await _service.GetByIdAsync(book.Id, _ownerId);

            // Assert
            Assert.NotNull(result);
            Assert.Null(result.BorrowerId);
            _bookRepoMock.Verify(r => r.GetByIdForOwnerAsync(book.Id, _ownerId), Times.Once);
        }
    }
}
