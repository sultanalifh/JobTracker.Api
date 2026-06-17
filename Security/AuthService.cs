using System.ComponentModel.DataAnnotations;
using JobTracker.Api.Dtos.Request;
using JobTracker.Api.Dtos.Response;
using JobTracker.Api.Exceptions;
using JobTracker.Api.Models;
using JobTracker.Api.Repositories;
using JobTracker.Api.Security.Models;
using Microsoft.OpenApi;

namespace JobTracker.Api.Security;

public class AuthService : IAuthService
{
    private readonly IUserRepositories _repositories;
    private readonly IPasswordService _passwordService;
    private readonly IJwtService _jwt;

    public AuthService(IUserRepositories repositories, IPasswordService passwordService, IJwtService jwt)
    {
        _repositories = repositories;
        _passwordService = passwordService;
        _jwt = jwt;
    }   
    public async Task<UserResponse> Register(RegisterRequest request)
    {
        string username = request.Username;
        string password = request.Password;

        if(string.IsNullOrWhiteSpace(username) || string.IsNullOrWhiteSpace(password))
        {
            throw new ValidationException("All field must be filled!");
        }

        bool usernameExists = await _repositories.ExistsByUsername(request.Username);

        if (usernameExists)
        {
            throw new UsernameAlreadyExistsException();
        }

        User user = new User()
        {
            Username = username,
            Role = UserRole.User,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };

        user.PasswordHash = _passwordService.HashPassword(user, password);

        await _repositories.AddAsync(user);

        await _repositories.SavesChangesAsync();

        return new UserResponse()
        {
            Id = user.Id,
            Username = user.Username,
            Role = user.Role.GetDisplayName()
        };
    }
    public async Task<LoginResponse> Login(LoginRequest request)
    {
        string username = request.Username;
        string password = request.Password;

        if(string.IsNullOrWhiteSpace(username) || string.IsNullOrWhiteSpace(password))
        {
            throw new ValidationException("All field must be filled!");
        }

        User? intendedUser = await _repositories.GetByUsernameAsync(username);

        if (intendedUser == null)
        {
            throw new InvalidCredentialsException();
        }

        bool passwordMatch = _passwordService.VerifyPassword(intendedUser!, intendedUser!.PasswordHash, password);

        if (!passwordMatch)
        {
            throw new InvalidCredentialsException();
        }

        JwtTokenResult tokenResult = _jwt.GenerateToken(intendedUser!);

        return new LoginResponse()
        {
            Token = tokenResult.Token,
            ExpiresAt = tokenResult.ExpiresAt,
            User = new UserResponse()
            {
                Id = intendedUser.Id,
                Username = intendedUser.Username,
                Role = intendedUser.Role.GetDisplayName()
            }
        };
    }
    public Task<bool> Logout()
    {
        throw new NotImplementedException();
    }
}