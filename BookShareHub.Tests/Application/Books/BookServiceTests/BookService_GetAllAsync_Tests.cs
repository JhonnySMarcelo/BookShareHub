using BookShareHub.Application.Books.Services;
using BookShareHub.Domain.Books.Entities;
using BookShareHub.Domain.Books.Repositories;
using Moq;

namespace BookShareHub.Tests.Application.Books.BookServiceTests
{
    public class BookService_GetAllAsync_Tests
    {
        private readonly Mock<IBookRepository> _bookRepoMock;
        private readonly BookService _service;

        public BookService_GetAllAsync_Tests()
        {
            _bookRepoMock = new Mock<IBookRepository>();
            _service = new BookService(_bookRepoMock.Object);
        }

        [Fact]
        public async Task Should_Return_All_Books_When_Repository_Has_Data()
        {
            // Arrange
            var books = new List<Book>
            {
                new Book("Clean Code", "Robert Martin", "Classic", true, Guid.NewGuid()),
                new Book("DDD", "Eric Evans", "Blue book", false, Guid.NewGuid())
            };


            _bookRepoMock.Setup(r => r.GetAllAsync())
                         .ReturnsAsync(books);

            // Act
            var result = await _service.GetAllAsync(null);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(2, result.Count());
            _bookRepoMock.Verify(r => r.GetAllAsync(), Times.Once);
        }

        [Fact]
        public async Task Should_Return_Empty_List_When_No_Books_Found()
        {
            // Arrange
            _bookRepoMock.Setup(r => r.GetAllAsync())
                         .ReturnsAsync(new List<Book>());

            // Act
            var result = await _service.GetAllAsync(null);

            // Assert
            Assert.NotNull(result);
            Assert.Empty(result);
            _bookRepoMock.Verify(r => r.GetAllAsync(), Times.Once);
        }
    }
}
