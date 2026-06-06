using BookShareHub.Application.Books.DTOs;
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

        public async Task<Book> CreateAsync(CreateBookDto dto)
        {
            if (dto == null)
                throw new ArgumentNullException(nameof(dto));

            var book = new Book(dto.Title, dto.Author, dto.Description, dto.Available, dto.OwnerId);

            await _bookRepository.AddAsync(book);

            return book;
        }      

        public async Task<Book?> GetByIdAsync(Guid id, Guid userId)
        {
            if (id == Guid.Empty)
                throw new ArgumentException("Id cannot be empty.", nameof(id));

            return await _bookRepository.GetByIdForOwnerAsync(id, userId);
        }

        public async Task<List<GetBookDto>> GetAllAsync(Guid? currentUserId)
        {
            var books = await _bookRepository.GetAllAsync();

            return books.Select(b => new GetBookDto
            {
                Id = b.Id,
                Title = b.Title,
                Author = b.Author,
                Description = b.Description,
                Available = b.Available,
                IsOwner = currentUserId.HasValue &&
                          b.OwnerId == currentUserId.Value
            })
            .OrderByDescending(x => x.IsOwner)
            .ThenBy(x => x.Title)
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

        public async Task<GetBookDto?> PatchAsync(Guid id, UpdateBookDto dto, Guid userId)
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

            return new GetBookDto
            {
                Id = updatedBook.Id,
                Title = updatedBook.Title,
                Author = updatedBook.Author,
                Description = updatedBook.Description,
                Available = updatedBook.Available,
                IsOwner = true
            };
        }
    }
}
