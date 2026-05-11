using BookShareHub.Application.Books.Services;
using BookShareHub.Application.Books.DTOs;
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
            var dto = new CreateBookDto
            {
                Title = "Clean Architecture",
                Author = "Robert Martin",
                Description = "Building upon the success of...",
                Available = true,
                OwnerId = Guid.NewGuid()
            };

            _bookRepoMock.Setup(r => r.AddAsync(It.IsAny<Book>()))
                         .Returns(Task.CompletedTask);

            // Act
            var result = await _service.CreateAsync(dto);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(dto.Title, result.Title);
            Assert.Equal(dto.Author, result.Author);
            _bookRepoMock.Verify(r => r.AddAsync(It.IsAny<Book>()), Times.Once);
        }

        [Fact]
        public async Task Should_Create_Book_When_Description_Is_Null()
        {
            // Arrange
            var dto = new CreateBookDto
            {
                Title = "Book",
                Author = "Author",
                OwnerId = Guid.NewGuid(),
                Available = true,
                Description = null
            };

            _bookRepoMock.Setup(r => r.AddAsync(It.IsAny<Book>()))
                         .Returns(Task.CompletedTask);

            // Act
            var result = await _service.CreateAsync(dto);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(dto.Title, result.Title);
            Assert.Equal(dto.Author, result.Author);
            Assert.Null(result.Description);
            _bookRepoMock.Verify(r => r.AddAsync(It.IsAny<Book>()), Times.Once);
        }

        [Theory]
        [InlineData("")]
        [InlineData("   ")]
        public async Task Should_Throw_ArgumentException_When_Title_Is_Invalid(string invalidTitle)
        {
            // Arrange
            var dto = new CreateBookDto
            {
                Title = invalidTitle,
                Author = "Author",
                OwnerId = Guid.NewGuid(),
                Available = true
            };

            _bookRepoMock.Setup(r => r.AddAsync(It.IsAny<Book>()))
                         .Returns(Task.CompletedTask);

            // Act & Assert
            await Assert.ThrowsAsync<ArgumentException>(() => _service.CreateAsync(dto));

            _bookRepoMock.Verify(r => r.AddAsync(It.IsAny<Book>()), Times.Never);
        }

        [Theory]
        [InlineData("")]
        [InlineData("   ")]
        public async Task Should_Throw_ArgumentException_When_Author_Is_Invalid(string invalidAuthor)
        {
            // Arrange
            var dto = new CreateBookDto
            {
                Title = "Valid Title",
                Author = invalidAuthor,
                OwnerId = Guid.NewGuid(),
                Available = true
            };

            _bookRepoMock.Setup(r => r.AddAsync(It.IsAny<Book>()))
                         .Returns(Task.CompletedTask);

            // Act & Assert
            await Assert.ThrowsAsync<ArgumentException>(() => _service.CreateAsync(dto));

            _bookRepoMock.Verify(r => r.AddAsync(It.IsAny<Book>()), Times.Never);
        }

        [Fact]
        public async Task Should_Throw_ArgumentException_When_OwnerId_Is_Empty()
        {
            // Arrange
            var dto = new CreateBookDto
            {
                Title = "Valid Title",
                Author = "Valid Author",
                OwnerId = Guid.Empty,
                Available = true
            };

            _bookRepoMock.Setup(r => r.AddAsync(It.IsAny<Book>()))
                         .Returns(Task.CompletedTask);

            // Act & Assert
            await Assert.ThrowsAsync<ArgumentException>(() => _service.CreateAsync(dto));

            _bookRepoMock.Verify(r => r.AddAsync(It.IsAny<Book>()), Times.Never);
        }

        [Fact]
        public async Task Should_Throw_ArgumentOutOfRangeException_When_Title_Exceeds_MaxLength()
        {
            // Arrange
            var dto = new CreateBookDto
            {
                Title = new string('A', 260),
                Author = "Author",
                OwnerId = Guid.NewGuid(),
                Available = true
            };

            _bookRepoMock.Setup(r => r.AddAsync(It.IsAny<Book>()))
                 .Returns(Task.CompletedTask);

            // Act & Assert
            await Assert.ThrowsAsync<ArgumentOutOfRangeException>(() => _service.CreateAsync(dto));

            _bookRepoMock.Verify(r => r.AddAsync(It.IsAny<Book>()), Times.Never);
        }

        [Fact]
        public async Task Should_Throw_ArgumentOutOfRangeException_When_Author_Exceeds_MaxLength()
        {
            // Arrange
            var dto = new CreateBookDto
            {
                Title = "Valid Title",
                Author = new string('B', 160),
                OwnerId = Guid.NewGuid(),
                Available = true
            };

            _bookRepoMock.Setup(r => r.AddAsync(It.IsAny<Book>()))
                         .Returns(Task.CompletedTask);

            // Act & Assert
            await Assert.ThrowsAsync<ArgumentOutOfRangeException>(() => _service.CreateAsync(dto));

            _bookRepoMock.Verify(r => r.AddAsync(It.IsAny<Book>()), Times.Never);
        }

        [Fact]
        public async Task Should_Throw_ArgumentOutOfRangeException_When_Description_Exceeds_MaxLength()
        {
            // Arrange
            var dto = new CreateBookDto
            {
                Title = "Valid Title",
                Author = "Valid Author",
                OwnerId = Guid.NewGuid(),
                Available = true,
                Description = new string('C', 4001)
            };

            _bookRepoMock.Setup(r => r.AddAsync(It.IsAny<Book>()))
                         .Returns(Task.CompletedTask);

            // Act & Assert
            await Assert.ThrowsAsync<ArgumentOutOfRangeException>(() => _service.CreateAsync(dto));

            _bookRepoMock.Verify(r => r.AddAsync(It.IsAny<Book>()), Times.Never);
        }
    }
}