using BookShareHub.Application.Books.DTOs;
using BookShareHub.Domain.Books.Entities;
using BookShareHub.Domain.Books.Repositories;
using System;
using System.Collections.Generic;
using System.Text;

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

        public async Task<Book?> GetByIdAsync(Guid id)
        {
            if (id == Guid.Empty)
                throw new ArgumentException("Id cannot be empty.", nameof(id));

            return await _bookRepository.GetByIdAsync(id);
        }

        public async Task<List<Book>> GetAllAsync()
        {
            var books = await _bookRepository.GetAllAsync();

            return books?.ToList() ?? new List<Book>();
        }

    }
}
