using BookShareHub.Application.Books.DTOs.Requests;
using BookShareHub.Application.Books.DTOs.Responses;
using BookShareHub.Application.Books.Mappers;
using BookShareHub.Domain.Books.Entities;
using BookShareHub.Domain.Books.Repositories;

namespace BookShareHub.Application.Books.Services
{
    public class BookService
    {
        private readonly IBookRepository _bookRepository;

        public BookService(IBookRepository bookRepository)
        {
            _bookRepository = bookRepository ?? throw new ArgumentNullException(nameof(bookRepository));
        }

        public async Task<BookResponse> CreateAsync(CreateBookRequest dto, Guid userId)
        {
            if (dto == null)
                throw new ArgumentNullException(nameof(dto));

            var book = new Book(
                   dto.Title,
                   dto.Author,
                   dto.Description,
                   dto.Available,
                   userId);

            await _bookRepository.AddAsync(book);

            return BookMapper.ToResponse(book, userId);
        }      

        public async Task<BookResponse?> GetByIdAsync(Guid id, Guid userId)
        {
            if (id == Guid.Empty)
                throw new ArgumentException("Id cannot be empty.", nameof(id));

            var book = await _bookRepository.GetByIdForOwnerAsync(id, userId);

            if (book == null)
                return null;

            return BookMapper.ToResponse(book, userId);
        }

        public async Task<List<BookResponse>> GetAllAsync(Guid? userId)
        {
            var books = await _bookRepository.GetAllAsync();

            return books
                .Select(b => BookMapper.ToResponse(b, userId))
                .OrderByDescending(b => b.IsOwner)
                .ThenBy(b => b.Title)
                .ToList();
        }

        public async Task<bool?> DeleteAsync(Guid id, Guid userId)
        {
            if (id == Guid.Empty)
                throw new ArgumentException("Book Id cannot be empty.", nameof(id));

            var book = await _bookRepository.GetByIdForOwnerAsync(id, userId);
            if (book == null) return null;

            if (!book.Available)
                throw new InvalidOperationException("Book is currently borrowed and cannot be deleted.");

            await _bookRepository.DeleteAsync(id, userId);

            return true;
        }

        public async Task<BookResponse?> PatchAsync(Guid id, PatchBookRequest dto, Guid userId)
        {
            if (id == Guid.Empty)
                throw new ArgumentException("Book Id cannot be empty.", nameof(id));

            if (dto == null)
                throw new ArgumentNullException(nameof(dto));

            var book = await _bookRepository.GetByIdForOwnerAsync(id, userId);
            if (book == null)
                return null;

            if (dto.Title != null)
                book.UpdateTitle(dto.Title);

            if (dto.Author != null)
                book.UpdateAuthor(dto.Author);

            if (dto.Description != null)
                book.UpdateDescription(dto.Description);

            if (dto.Available.HasValue)
                book.UpdateAvailability(dto.Available.Value);

            var updatedBook = await _bookRepository.PatchAsync(book, userId);
            if (updatedBook == null)
                return null;

            return BookMapper.ToResponse(updatedBook, userId);
        }
    }
}
