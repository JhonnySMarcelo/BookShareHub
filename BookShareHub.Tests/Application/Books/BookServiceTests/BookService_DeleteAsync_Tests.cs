using BookShareHub.Application.Books.Services;
using BookShareHub.Domain.Books.Entities;
using BookShareHub.Domain.Books.Repositories;
using Moq;

namespace BookShareHub.Tests.Application.Books.BookServiceTests
{
    public class BookService_DeleteAsync_Tests
    {
        private readonly Mock<IBookRepository> _bookRepoMock;
        private readonly BookService _service;

        public BookService_DeleteAsync_Tests()
        {
            _bookRepoMock = new Mock<IBookRepository>();
            _service = new BookService(_bookRepoMock.Object);
        }

        [Fact]
        public async Task Should_Return_True_When_Book_Is_Deleted()
        {
            // Arrange
            var book = new Book("Clean Code", "Robert Martin", "Classic", true, Guid.NewGuid());
            _bookRepoMock.Setup(r => r.GetByIdAsync(book.Id)).ReturnsAsync(book);
            _bookRepoMock.Setup(r => r.DeleteAsync(book.Id)).ReturnsAsync(true);

            // Act
            var result = await _service.DeleteAsync(book.Id);

            // Assert
            Assert.True(result);
            _bookRepoMock.Verify(r => r.DeleteAsync(book.Id), Times.Once);
        }

        [Fact]
        public async Task Should_Return_Null_When_Book_Not_Found()
        {
            // Arrange
            var id = Guid.NewGuid();
            _bookRepoMock.Setup(r => r.GetByIdAsync(id)).ReturnsAsync((Book?)null);

            // Act
            var result = await _service.DeleteAsync(id);

            // Assert
            Assert.Null(result);
            _bookRepoMock.Verify(r => r.DeleteAsync(It.IsAny<Guid>()), Times.Never);
        }

        [Fact]
        public async Task Should_Throw_ArgumentException_When_Id_Is_Empty()
        {
            // Arrange
            var emptyId = Guid.Empty;

            // Act & Assert
            await Assert.ThrowsAsync<ArgumentException>(() => _service.DeleteAsync(emptyId));

            _bookRepoMock.Verify(r => r.GetByIdAsync(It.IsAny<Guid>()), Times.Never);
            _bookRepoMock.Verify(r => r.DeleteAsync(It.IsAny<Guid>()), Times.Never);
        }

        [Fact]
        public async Task Should_Throw_InvalidOperationException_When_Book_Is_Not_Available()
        {
            // Arrange
            var book = new Book("DDD", "Eric Evans", "Blue book", false, Guid.NewGuid());
            _bookRepoMock.Setup(r => r.GetByIdAsync(book.Id)).ReturnsAsync(book);

            // Act & Assert
            await Assert.ThrowsAsync<InvalidOperationException>(() => _service.DeleteAsync(book.Id));

            _bookRepoMock.Verify(r => r.DeleteAsync(It.IsAny<Guid>()), Times.Never);
        }
    }
}
