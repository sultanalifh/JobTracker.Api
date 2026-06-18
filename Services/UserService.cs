using JobTracker.Api.Dtos.Response;
using JobTracker.Api.Models;
using JobTracker.Api.Repositories;
using Microsoft.OpenApi;

namespace JobTracker.Api.Services;

public class UserService : IUserService
{
    private readonly IUserRepositories _repositories;

    public UserService(IUserRepositories repositories)
    {
        _repositories = repositories;
    }
    public async Task<List<UserResponse>> GetAllUsersAsync()
    {
        List<User> users = await _repositories.GetAllAsync();

        List<UserResponse> userResponses = users.Select(user => new UserResponse()
        {
            Id = user.Id,
            Username = user.Username,
            Role = user.Role.GetDisplayName()
        }).ToList();

        return userResponses;
    }
}