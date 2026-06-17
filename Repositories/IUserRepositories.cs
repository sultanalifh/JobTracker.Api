using JobTracker.Api.Models;

namespace JobTracker.Api.Repositories;

public interface IUserRepositories : IRepositories
{
    Task<List<User>> GetAllAsync();
    Task<User?> GetByIdAsync(long id);
    Task<User?> GetByUsernameAsync(string username);
    Task<bool> ExistsByUsername(string username);
    Task AddAsync(User user);
    Task RemoveAsync(User user);
}