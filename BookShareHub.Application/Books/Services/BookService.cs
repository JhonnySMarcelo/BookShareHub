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
            _bookRepository = bookRepository;
        }

        public async Task<Book> CreateAsync(CreateBookDto dto)
        {
            var book = new Book
            {
                Id = Guid.NewGuid(),
                Title = dto.Title,
                Author = dto.Author,
                Description = dto.Description,
                Available = dto.Available
            };

            await _bookRepository.AddAsync(book);
            await _bookRepository.SaveChangesAsync();

            return book;
        }
    }
}
