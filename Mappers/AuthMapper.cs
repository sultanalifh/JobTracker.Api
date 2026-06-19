using JobTracker.Api.Dtos.Request;
using JobTracker.Api.Dtos.Response;
using JobTracker.Api.Models;
using Microsoft.OpenApi;

namespace JobTracker.Api.Mappers;

public static class AuthMapper
{
    public static User CreateUser(this RegisterRequest request)
        => new User()
        {
            Username = request.Username,
            Role = UserRole.User,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };

    public static UserResponse ToResponse(this User user)
        => new UserResponse()
        {
            Id = user.Id,
            Username = user.Username,
            Role = user.Role.GetDisplayName()
        };
    
    public static LoginResponse ToLoginResponse(this UserResponse response, string token, DateTimeOffset date)
        => new LoginResponse()
        {
            Token = token,
            ExpiresAt = date,
            User = response
        };
}