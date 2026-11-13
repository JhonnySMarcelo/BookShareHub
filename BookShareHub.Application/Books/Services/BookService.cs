using BookShareHub.Application.Books.DTOs.Requests;
using BookShareHub.Application.Books.DTOs.Responses;
using BookShareHub.Application.Books.Exceptions;
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

            Book book;
            try
            {
                book = new Book(
                    dto.Title,
                    dto.Author,
                    dto.Description,
                    dto.Available,
                    userId);
            }
            catch (ArgumentException exception) when (exception.ParamName == "title")
            {
                throw ToBookValidationException(exception, nameof(CreateBookRequest.Title));
            }
            catch (ArgumentException exception) when (exception.ParamName == "author")
            {
                throw ToBookValidationException(exception, nameof(CreateBookRequest.Author));
            }
            catch (ArgumentException exception) when (exception.ParamName == "description")
            {
                throw ToBookValidationException(exception, nameof(CreateBookRequest.Description));
            }

            await _bookRepository.AddAsync(book);

            return BookMapper.ToResponse(book, userId);
        }      

        public async Task<BookResponse?> GetByIdAsync(Guid id, Guid userId)
        {
            ThrowIfIdIsEmpty(id);

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
            ThrowIfIdIsEmpty(id);

            var book = await _bookRepository.GetByIdForOwnerAsync(id, userId);
            if (book == null) return null;

            if (!book.Available)
                throw new BookUnavailableException();

            await _bookRepository.DeleteAsync(id, userId);

            return true;
        }

        public async Task<BookResponse?> PatchAsync(Guid id, PatchBookRequest dto, Guid userId)
        {
            ThrowIfIdIsEmpty(id);

            if (dto == null)
                throw new ArgumentNullException(nameof(dto));

            var book = await _bookRepository.GetByIdForOwnerAsync(id, userId);
            if (book == null)
                return null;

            if (dto.Title != null)
            {
                try
                {
                    book.UpdateTitle(dto.Title);
                }
                catch (ArgumentException exception) when (exception.ParamName == "title")
                {
                    throw ToBookValidationException(exception, nameof(PatchBookRequest.Title));
                }
            }

            if (dto.Author != null)
            {
                try
                {
                    book.UpdateAuthor(dto.Author);
                }
                catch (ArgumentException exception) when (exception.ParamName == "author")
                {
                    throw ToBookValidationException(exception, nameof(PatchBookRequest.Author));
                }
            }

            if (dto.Description != null)
            {
                try
                {
                    book.UpdateDescription(dto.Description);
                }
                catch (ArgumentException exception) when (exception.ParamName == "description")
                {
                    throw ToBookValidationException(exception, nameof(PatchBookRequest.Description));
                }
            }

            if (dto.Available.HasValue)
                book.UpdateAvailability(dto.Available.Value);

            var updatedBook = await _bookRepository.PatchAsync(book, userId);
            if (updatedBook == null)
                return null;

            return BookMapper.ToResponse(updatedBook, userId);
        }

        private static void ThrowIfIdIsEmpty(Guid id)
        {
            if (id == Guid.Empty)
            {
                throw new BookValidationException(new Dictionary<string, string[]>
                {
                    [nameof(id)] = ["Book Id cannot be empty."]
                });
            }
        }

        private static BookValidationException ToBookValidationException(ArgumentException exception, string field)
        {
            return new BookValidationException(new Dictionary<string, string[]>
            {
                [field] = [exception.Message]
            });
        }
    }
}
