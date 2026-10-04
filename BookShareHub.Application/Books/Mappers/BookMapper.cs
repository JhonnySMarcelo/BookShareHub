using BookShareHub.Application.Books.DTOs.Responses;
using BookShareHub.Domain.Books.Entities;

namespace BookShareHub.Application.Books.Mappers
{
    public static class BookMapper
    {
        public static BookResponse ToResponse(
            Book book,
            Guid? currentUserId = null)
        {
            return new BookResponse
            {
                Id = book.Id,
                Title = book.Title,
                Author = book.Author,
                Description = book.Description,
                Available = book.Available,
                IsOwner = currentUserId.HasValue &&
                          book.OwnerId == currentUserId.Value
            };
        }
    }
}
