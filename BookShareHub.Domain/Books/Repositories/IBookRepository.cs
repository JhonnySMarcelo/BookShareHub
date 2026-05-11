using BookShareHub.Domain.Books.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace BookShareHub.Domain.Books.Repositories
{
    public interface IBookRepository
    {
        Task AddAsync(Book book);
        Task<Book?> GetByIdAsync(Guid id);
    }
}
