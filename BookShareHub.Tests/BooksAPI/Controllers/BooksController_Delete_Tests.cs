using BookShareHub.Application.Books.Exceptions;
using BookShareHub.Application.Books.Services;
using BookShareHub.Application.Users.Interfaces;
using BookShareHub.BooksAPI.Controllers;
using BookShareHub.Domain.Books.Entities;
using BookShareHub.Domain.Books.Repositories;
using Microsoft.AspNetCore.Mvc;
using Moq;

namespace BookShareHub.Tests.BooksAPI.Controllers
{
    public class BooksController_Delete_Tests
    {
        private readonly Mock<IBookRepository> _bookRepository = new();
        private readonly Guid _userId = Guid.NewGuid();

        private BooksController CreateController()
        {
            var currentUser = new Mock<ICurrentUser>();
            currentUser.SetupGet(user => user.UserId).Returns(_userId);

            return new BooksController(new BookService(_bookRepository.Object), currentUser.Object);
        }

        [Fact]
        public async Task Should_Propagate_BookValidationException_When_Id_Is_Empty()
        {
            // Arrange
            var controller = CreateController();

            // Act
            var exception = await Assert.ThrowsAsync<BookValidationException>(() => controller.Delete(Guid.Empty));

            // Assert
            Assert.Contains("id", exception.Errors.Keys);

            _bookRepository.Verify(
                repository => repository.GetByIdForOwnerAsync(It.IsAny<Guid>(), It.IsAny<Guid>()),
                Times.Never);
        }

        [Fact]
        public async Task Should_Return_404_When_Book_Does_Not_Exist()
        {
            // Arrange
            var bookId = Guid.NewGuid();
            _bookRepository
                .Setup(repository => repository.GetByIdForOwnerAsync(bookId, _userId))
                .ReturnsAsync((Book?)null);
            var controller = CreateController();

            // Act
            var result = await controller.Delete(bookId);

            // Assert
            Assert.IsType<NotFoundResult>(result);
            _bookRepository.Verify(
                repository => repository.DeleteAsync(It.IsAny<Guid>(), It.IsAny<Guid>()),
                Times.Never);
        }

        [Fact]
        public async Task Should_Return_204_When_Book_Is_Deleted()
        {
            // Arrange
            var book = new Book("Clean Code", "Robert Martin", "Classic", true, _userId);
            _bookRepository
                .Setup(repository => repository.GetByIdForOwnerAsync(book.Id, _userId))
                .ReturnsAsync(book);
            _bookRepository
                .Setup(repository => repository.DeleteAsync(book.Id, _userId))
                .ReturnsAsync(true);
            var controller = CreateController();

            // Act
            var result = await controller.Delete(book.Id);

            // Assert
            Assert.IsType<NoContentResult>(result);
            _bookRepository.Verify(repository => repository.DeleteAsync(book.Id, _userId), Times.Once);
        }
    }
}
