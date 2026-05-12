using BookShareHub.Domain.Users.Entities;

namespace BookShareHub.Domain.Users.Repositories
{
    public interface IUserRepository
    {
        Task AddAsync(User user);
        Task<User?> FindAsync(Dictionary<string, object> filters);
        Task<User?> PatchAsync(User user);
        Task<bool> DeleteAsync(Guid id);
    }
}
