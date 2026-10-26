using BookShareHub.Application.Books.DTOs.Requests;
using BookShareHub.Application.Books.Services;
using BookShareHub.Domain.Books.Entities;
using BookShareHub.Domain.Books.Repositories;
using Moq;

namespace BookShareHub.Tests.Application.Books.BookServiceTests
{
    public class BookService_CreateAsync_Tests
    {
        private readonly Mock<IBookRepository> _bookRepoMock;
        private readonly BookService _service;

        public BookService_CreateAsync_Tests()
        {
            _bookRepoMock = new Mock<IBookRepository>();
            _service = new BookService(_bookRepoMock.Object);
        }

        [Fact]
        public async Task Should_Create_Book_When_Valid()
        {
            // Arrange
            var userId = Guid.NewGuid();

            var request = new CreateBookRequest
            {
                Title = "Clean Architecture",
                Author = "Robert Martin",
                Description = "Building upon the success of...",
                Available = true
            };

            _bookRepoMock
                .Setup(r => r.AddAsync(It.IsAny<Book>()))
                .Returns(Task.CompletedTask);

            // Act
            var result = await _service.CreateAsync(request, userId);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(request.Title, result.Title);
            Assert.Equal(request.Author, result.Author);
            Assert.Equal(request.Description, result.Description);
            Assert.True(result.Available);
            Assert.True(result.IsOwner);

            _bookRepoMock.Verify(
                r => r.AddAsync(It.Is<Book>(b =>
                    b.Title == request.Title &&
                    b.Author == request.Author &&
                    b.OwnerId == userId &&
                    b.Available == request.Available)),
                Times.Once);
        }

        [Fact]
        public async Task Should_Create_Book_When_Description_Is_Null()
        {
            // Arrange
            var userId = Guid.NewGuid();

            var request = new CreateBookRequest
            {
                Title = "Book",
                Author = "Author",
                Description = null,
                Available = true
            };

            _bookRepoMock
                .Setup(r => r.AddAsync(It.IsAny<Book>()))
                .Returns(Task.CompletedTask);

            // Act
            var result = await _service.CreateAsync(request, userId);

            // Assert
            Assert.Equal(request.Title, result.Title);
            Assert.Equal(request.Author, result.Author);
            Assert.Null(result.Description);
            Assert.True(result.IsOwner);

            _bookRepoMock.Verify(
                r => r.AddAsync(It.IsAny<Book>()),
                Times.Once);
        }

        [Theory]
        [InlineData("")]
        [InlineData("   ")]
        public async Task Should_Throw_ArgumentException_When_Title_Is_Invalid(
            string invalidTitle)
        {
            // Arrange
            var request = new CreateBookRequest
            {
                Title = invalidTitle,
                Author = "Author",
                Available = true
            };

            // Act & Assert
            await Assert.ThrowsAsync<ArgumentException>(
                () => _service.CreateAsync(request, Guid.NewGuid()));

            _bookRepoMock.Verify(
                r => r.AddAsync(It.IsAny<Book>()),
                Times.Never);
        }

        [Theory]
        [InlineData("")]
        [InlineData("   ")]
        public async Task Should_Throw_ArgumentException_When_Author_Is_Invalid(
            string invalidAuthor)
        {
            // Arrange
            var request = new CreateBookRequest
            {
                Title = "Valid Title",
                Author = invalidAuthor,
                Available = true
            };

            // Act & Assert
            await Assert.ThrowsAsync<ArgumentException>(
                () => _service.CreateAsync(request, Guid.NewGuid()));

            _bookRepoMock.Verify(
                r => r.AddAsync(It.IsAny<Book>()),
                Times.Never);
        }

        [Fact]
        public async Task Should_Throw_ArgumentException_When_UserId_Is_Empty()
        {
            // Arrange
            var request = new CreateBookRequest
            {
                Title = "Valid Title",
                Author = "Valid Author",
                Available = true
            };

            // Act & Assert
            await Assert.ThrowsAsync<ArgumentException>(
                () => _service.CreateAsync(request, Guid.Empty));

            _bookRepoMock.Verify(
                r => r.AddAsync(It.IsAny<Book>()),
                Times.Never);
        }

        [Fact]
        public async Task Should_Throw_ArgumentOutOfRangeException_When_Title_Exceeds_MaxLength()
        {
            // Arrange
            var request = new CreateBookRequest
            {
                Title = new string('A', 260),
                Author = "Author",
                Available = true
            };

            // Act & Assert
            await Assert.ThrowsAsync<ArgumentOutOfRangeException>(
                () => _service.CreateAsync(request, Guid.NewGuid()));

            _bookRepoMock.Verify(
                r => r.AddAsync(It.IsAny<Book>()),
                Times.Never);
        }

        [Fact]
        public async Task Should_Throw_ArgumentOutOfRangeException_When_Author_Exceeds_MaxLength()
        {
            // Arrange
            var request = new CreateBookRequest
            {
                Title = "Valid Title",
                Author = new string('B', 160),
                Available = true
            };

            // Act & Assert
            await Assert.ThrowsAsync<ArgumentOutOfRangeException>(
                () => _service.CreateAsync(request, Guid.NewGuid()));

            _bookRepoMock.Verify(
                r => r.AddAsync(It.IsAny<Book>()),
                Times.Never);
        }

        [Fact]
        public async Task Should_Throw_ArgumentOutOfRangeException_When_Description_Exceeds_MaxLength()
        {
            // Arrange
            var request = new CreateBookRequest
            {
                Title = "Valid Title",
                Author = "Valid Author",
                Available = true,
                Description = new string('C', 4001)
            };

            // Act & Assert
            await Assert.ThrowsAsync<ArgumentOutOfRangeException>(
                () => _service.CreateAsync(request, Guid.NewGuid()));

            _bookRepoMock.Verify(
                r => r.AddAsync(It.IsAny<Book>()),
                Times.Never);
        }
    }
}
