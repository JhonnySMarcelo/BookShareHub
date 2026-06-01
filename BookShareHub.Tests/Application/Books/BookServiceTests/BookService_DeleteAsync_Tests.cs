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
        private readonly Guid _ownerId;

        public BookService_DeleteAsync_Tests()
        {
            _bookRepoMock = new Mock<IBookRepository>();
            _service = new BookService(_bookRepoMock.Object);
            _ownerId = Guid.NewGuid();
        }

        [Fact]
        public async Task Should_Return_True_When_Book_Is_Deleted()
        {
            // Arrange
            var book = new Book("Clean Code", "Robert Martin", "Classic", true, Guid.NewGuid());
            _bookRepoMock.Setup(r => r.GetByIdForOwnerAsync(book.Id, _ownerId)).ReturnsAsync(book);
            _bookRepoMock.Setup(r => r.DeleteAsync(book.Id, _ownerId)).ReturnsAsync(true);

            // Act
            var result = await _service.DeleteAsync(book.Id, _ownerId);

            // Assert
            Assert.True(result);
            _bookRepoMock.Verify(r => r.DeleteAsync(book.Id, _ownerId), Times.Once);
        }

        [Fact]
        public async Task Should_Return_Null_When_Book_Not_Found()
        {
            // Arrange
            var id = Guid.NewGuid();
            _bookRepoMock.Setup(r => r.GetByIdForOwnerAsync(id, _ownerId)).ReturnsAsync((Book?)null);

            // Act
            var result = await _service.DeleteAsync(id, _ownerId);

            // Assert
            Assert.Null(result);
            _bookRepoMock.Verify(r => r.DeleteAsync(It.IsAny<Guid>(), _ownerId), Times.Never);
        }

        [Fact]
        public async Task Should_Throw_ArgumentException_When_Id_Is_Empty()
        {
            // Arrange
            var emptyId = Guid.Empty;

            // Act & Assert
            await Assert.ThrowsAsync<ArgumentException>(() => _service.DeleteAsync(emptyId, _ownerId));

            _bookRepoMock.Verify(r => r.GetByIdForOwnerAsync(It.IsAny<Guid>(), _ownerId), Times.Never);
            _bookRepoMock.Verify(r => r.DeleteAsync(It.IsAny<Guid>(), _ownerId), Times.Never);
        }

        [Fact]
        public async Task Should_Throw_InvalidOperationException_When_Book_Is_Not_Available()
        {
            // Arrange
            var book = new Book("DDD", "Eric Evans", "Blue book", false, Guid.NewGuid());
            _bookRepoMock.Setup(r => r.GetByIdForOwnerAsync(book.Id, _ownerId)).ReturnsAsync(book);

            // Act & Assert
            await Assert.ThrowsAsync<InvalidOperationException>(() => _service.DeleteAsync(book.Id, _ownerId));

            _bookRepoMock.Verify(r => r.DeleteAsync(It.IsAny<Guid>(), _ownerId), Times.Never);
        }
    }
}
