using BookShareHub.Application.Books.DTOs;
using BookShareHub.Application.Books.Services;
using BookShareHub.Domain.Books.Entities;
using BookShareHub.Domain.Books.Repositories;
using Moq;

namespace BookShareHub.Tests.Application.Books.BookServiceTests
{
    public class BookService_PatchAsync_Tests
    {
        private readonly Mock<IBookRepository> _bookRepoMock;
        private readonly BookService _service;

        public BookService_PatchAsync_Tests()
        {
            _bookRepoMock = new Mock<IBookRepository>();
            _service = new BookService(_bookRepoMock.Object);
        }

        [Fact]
        public async Task Should_Patch_Title_Only_When_Provided()
        {
            // Arrange
            var book = new Book("Old Title", "Author", "Desc", true, Guid.NewGuid());
            _bookRepoMock.Setup(r => r.GetByIdAsync(book.Id)).ReturnsAsync(book);
            var dto = new UpdateBookDto { Title = "New Title" };

            // Act
            var result = await _service.PatchAsync(book.Id, dto);

            // Assert
            Assert.Equal("New Title", result.Title);
            Assert.Equal("Author", result.Author);
            Assert.Equal("Desc", result.Description);
            Assert.True(result.Available);
            _bookRepoMock.Verify(r => r.PatchAsync(book), Times.Once);
        }

        [Fact]
        public async Task Should_Patch_Author_Only_When_Provided()
        {
            // Arrange
            var book = new Book("Title", "Old Author", "Desc", true, Guid.NewGuid());
            _bookRepoMock.Setup(r => r.GetByIdAsync(book.Id)).ReturnsAsync(book);
            var dto = new UpdateBookDto { Author = "New Author" };

            // Act
            var result = await _service.PatchAsync(book.Id, dto);

            // Assert
            Assert.Equal("Title", result.Title);
            Assert.Equal("New Author", result.Author);
            Assert.Equal("Desc", result.Description);
            Assert.True(result.Available);
            _bookRepoMock.Verify(r => r.PatchAsync(book), Times.Once);
        }

        [Fact]
        public async Task Should_Patch_Description_Only_When_Provided()
        {
            // Arrange
            var book = new Book("Title", "Author", "Old Desc", true, Guid.NewGuid());
            _bookRepoMock.Setup(r => r.GetByIdAsync(book.Id)).ReturnsAsync(book);
            var dto = new UpdateBookDto { Description = "New Desc" };

            // Act
            var result = await _service.PatchAsync(book.Id, dto);

            // Assert
            Assert.Equal("Title", result.Title);
            Assert.Equal("Author", result.Author);
            Assert.Equal("New Desc", result.Description);
            Assert.True(result.Available);
            _bookRepoMock.Verify(r => r.PatchAsync(book), Times.Once);
        }

        [Fact]
        public async Task Should_Patch_Availability_Only_When_Provided()
        {
            // Arrange
            var book = new Book("Title", "Author", "Desc", true, Guid.NewGuid());
            _bookRepoMock.Setup(r => r.GetByIdAsync(book.Id)).ReturnsAsync(book);
            var dto = new UpdateBookDto { Available = false };

            // Act
            var result = await _service.PatchAsync(book.Id, dto);

            // Assert
            Assert.Equal("Title", result.Title);
            Assert.Equal("Author", result.Author);
            Assert.Equal("Desc", result.Description);
            Assert.False(result.Available);
            _bookRepoMock.Verify(r => r.PatchAsync(book), Times.Once);
        }

        [Fact]
        public async Task Should_Return_Null_When_Book_Not_Found()
        {
            // Arrange
            var id = Guid.NewGuid();
            _bookRepoMock.Setup(r => r.GetByIdAsync(id)).ReturnsAsync((Book?)null);
            var dto = new UpdateBookDto { Title = "New Title" };

            // Act
            var result = await _service.PatchAsync(id, dto);

            // Assert
            Assert.Null(result);
            _bookRepoMock.Verify(r => r.PatchAsync(It.IsAny<Book>()), Times.Never);
        }

        [Fact]
        public async Task Should_Throw_ArgumentException_When_Id_Is_Empty()
        {
            // Arrange
            var emptyId = Guid.Empty;
            var dto = new UpdateBookDto { Title = "New Title" };

            // Act & Assert
            await Assert.ThrowsAsync<ArgumentException>(() => _service.PatchAsync(emptyId, dto));

            _bookRepoMock.Verify(r => r.GetByIdAsync(It.IsAny<Guid>()), Times.Never);
            _bookRepoMock.Verify(r => r.PatchAsync(It.IsAny<Book>()), Times.Never);
        }

        [Fact]
        public async Task Should_Throw_ArgumentNullException_When_Dto_Is_Null()
        {
            // Arrange
            var id = Guid.NewGuid();

            // Act & Assert
            await Assert.ThrowsAsync<ArgumentNullException>(() => _service.PatchAsync(id, null));

            _bookRepoMock.Verify(r => r.GetByIdAsync(It.IsAny<Guid>()), Times.Never);
            _bookRepoMock.Verify(r => r.PatchAsync(It.IsAny<Book>()), Times.Never);
        }

        [Theory]
        [InlineData("")]
        [InlineData("   ")]
        public async Task Should_Throw_ArgumentException_When_Title_Is_Invalid_On_Patch(string invalidTitle)
        {
            // Arrange
            var book = new Book("Old Title", "Author", "Desc", true, Guid.NewGuid());
            _bookRepoMock.Setup(r => r.GetByIdAsync(book.Id)).ReturnsAsync(book);
            var dto = new UpdateBookDto { Title = invalidTitle };

            // Act & Assert
            await Assert.ThrowsAsync<ArgumentException>(() => _service.PatchAsync(book.Id, dto));

            _bookRepoMock.Verify(r => r.PatchAsync(It.IsAny<Book>()), Times.Never);
        }

        [Theory]
        [InlineData("")]
        [InlineData("   ")]
        public async Task Should_Throw_ArgumentException_When_Author_Is_Invalid_On_Patch(string invalidAuthor)
        {
            // Arrange
            var book = new Book("Title", "Old Author", "Desc", true, Guid.NewGuid());
            _bookRepoMock.Setup(r => r.GetByIdAsync(book.Id)).ReturnsAsync(book);
            var dto = new UpdateBookDto { Author = invalidAuthor };

            // Act & Assert
            await Assert.ThrowsAsync<ArgumentException>(() => _service.PatchAsync(book.Id, dto));

            _bookRepoMock.Verify(r => r.PatchAsync(It.IsAny<Book>()), Times.Never);
        }

        [Fact]
        public async Task Should_Throw_ArgumentOutOfRangeException_When_Title_Exceeds_MaxLength_On_Patch()
        {
            // Arrange
            var book = new Book("Valid Title", "Author", "Desc", true, Guid.NewGuid());
            _bookRepoMock.Setup(r => r.GetByIdAsync(book.Id)).ReturnsAsync(book);
            var dto = new UpdateBookDto { Title = new string('A', 260) };

            // Act & Assert
            await Assert.ThrowsAsync<ArgumentOutOfRangeException>(() => _service.PatchAsync(book.Id, dto));

            _bookRepoMock.Verify(r => r.PatchAsync(It.IsAny<Book>()), Times.Never);
        }

        [Fact]
        public async Task Should_Throw_ArgumentOutOfRangeException_When_Author_Exceeds_MaxLength_On_Patch()
        {
            // Arrange
            var book = new Book("Valid Title", "Valid Author", "Desc", true, Guid.NewGuid());
            _bookRepoMock.Setup(r => r.GetByIdAsync(book.Id)).ReturnsAsync(book);
            var dto = new UpdateBookDto { Author = new string('B', 160) };

            // Act & Assert
            await Assert.ThrowsAsync<ArgumentOutOfRangeException>(() => _service.PatchAsync(book.Id, dto));

            _bookRepoMock.Verify(r => r.PatchAsync(It.IsAny<Book>()), Times.Never);
        }

        [Fact]
        public async Task Should_Throw_ArgumentOutOfRangeException_When_Description_Exceeds_MaxLength_On_Patch()
        {
            // Arrange
            var book = new Book("Valid Title", "Valid Author", "Valid Desc", true, Guid.NewGuid());
            _bookRepoMock.Setup(r => r.GetByIdAsync(book.Id)).ReturnsAsync(book);
            var dto = new UpdateBookDto { Description = new string('C', 4001) };

            // Act & Assert
            await Assert.ThrowsAsync<ArgumentOutOfRangeException>(() => _service.PatchAsync(book.Id, dto));

            _bookRepoMock.Verify(r => r.PatchAsync(It.IsAny<Book>()), Times.Never);
        }
    }
}
