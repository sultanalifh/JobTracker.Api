using System.ComponentModel.DataAnnotations;
using JobTracker.Api.Dtos.Request;
using JobTracker.Api.Dtos.Response;
using JobTracker.Api.Exceptions;
using JobTracker.Api.Mappers;
using JobTracker.Api.Models;
using JobTracker.Api.Repositories;
using JobTracker.Api.Security.Models;
using JobTracker.Api.Validators;
using Microsoft.OpenApi;

namespace JobTracker.Api.Security;

public class AuthService : IAuthService
{
    private readonly IUserRepositories _repositories;
    private readonly IPasswordService _passwordService;
    private readonly IJwtService _jwt;
    private readonly IValidator<LoginRequest> _loginValidator;
    private readonly IValidator<RegisterRequest> _registerValidator;
    public AuthService(
        IUserRepositories repositories,
        IPasswordService passwordService,
        IJwtService jwt,
        IValidator<LoginRequest> loginValidator,
        IValidator<RegisterRequest> registerValidator)
    {
        _repositories = repositories;
        _passwordService = passwordService;
        _jwt = jwt;

        _loginValidator = loginValidator;
        _registerValidator = registerValidator;
    }   
    public async Task<UserResponse> Register(RegisterRequest request)
    {
        _registerValidator.Validate(request);

        bool usernameExists = await _repositories.ExistsByUsername(request.Username);

        if (usernameExists)
        {
            throw new UsernameAlreadyExistsException();
        }

        User user = request.CreateUser();

        user.PasswordHash = _passwordService.HashPassword(user, request.Password);

        await _repositories.AddAsync(user);

        await _repositories.SavesChangesAsync();

        return user.ToResponse();
    }
    public async Task<LoginResponse> Login(LoginRequest request)
    {
        _loginValidator.Validate(request);

        User? intendedUser = await _repositories.GetByUsernameAsync(request.Username);

        if (intendedUser == null)
        {
            throw new InvalidCredentialsException();
        }

        bool passwordMatch = _passwordService.VerifyPassword(intendedUser, intendedUser.PasswordHash, request.Password);

        if (!passwordMatch)
        {
            throw new InvalidCredentialsException();
        }

        JwtTokenResult tokenResult = _jwt.GenerateToken(intendedUser!);

        return intendedUser.ToResponse().ToLoginResponse(tokenResult.Token, tokenResult.ExpiresAt);
    }
    public Task<bool> Logout()
    {
        throw new NotImplementedException();
    }
}