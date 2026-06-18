using JobTracker.Api.Dtos.Request;
using JobTracker.Api.Dtos.Response;
using JobTracker.Api.Models;

namespace JobTracker.Api.Services;

public interface IUserService
{
    Task<List<UserResponse>> GetAllUsersAsync();
}