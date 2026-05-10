using BookShareHub.Application.Books.DTOs;
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

        public async Task<CreateBookDto?> CreateAsync(CreateBookDto dto)
        {
            throw new NotImplementedException();
        }
    }
}
