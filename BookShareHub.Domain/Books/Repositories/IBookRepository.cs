using BookShareHub.Domain.Books.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace BookShareHub.Domain.Books.Repositories
{
    public interface IBookRepository
    {
        Task AddAsync(Book book);
        Task<bool> DeleteAsync(Guid id, Guid userId);
        Task<List<Book>> GetAllAsync();
        Task<Book?> GetByIdForOwnerAsync(Guid id, Guid userId);
        Task<Book?> PatchAsync(Book book, Guid userId);
    }
}
