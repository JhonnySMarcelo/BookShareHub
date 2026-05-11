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
    }
}