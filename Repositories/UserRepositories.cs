using JobTracker.Api.Data;
using JobTracker.Api.Models;
using JobTracker.Api.Repositories;
using Microsoft.EntityFrameworkCore;

namespace JobTracker.Api.Repositories;

public class UserRepositories : IUserRepositories
{  
    private readonly AppDbContext _context;

    public UserRepositories(AppDbContext context)
    {
        _context = context;
    }
    public async Task<List<User>> GetAllAsync()
    {
        List<User> users = await _context.Users.ToListAsync();

        return users;
    }

    public async Task<User?> GetByIdAsync(long id)
    {
        User? user = await _context.Users.FirstOrDefaultAsync(user => user.Id == id);

        return user;
    }

    public async Task<User?> GetByUsernameAsync(string username)
    {
        User? user = await _context.Users.FirstOrDefaultAsync(user => user.Username == username);

        return user;
    }

    public async Task<bool> ExistsByUsername(string username)
    {
        User? user = await _context.Users.FirstOrDefaultAsync(user => user.Username == username);

        return user != null;
    }

    public async Task AddAsync(User user) => await _context.Users.AddAsync(user);

    public Task RemoveAsync(User user)
    {
        _context.Users.Remove(user);

        return Task.CompletedTask;
    }

    public async Task SavesChangesAsync() => await _context.SaveChangesAsync();
}