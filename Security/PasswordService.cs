using JobTracker.Api.Models;
using Microsoft.AspNetCore.Identity;

namespace JobTracker.Api.Security;

public class PasswordService : IPasswordService
{
    private readonly PasswordHasher<User> _hasher = new();

    public string HashPassword(User user, string password)
    {
        string hashedPassword = _hasher.HashPassword(user, password);

        return hashedPassword;
    }

    public bool VerifyPassword(User user, string hashedPassword, string password)
    {
        var result = _hasher.VerifyHashedPassword(user, hashedPassword, password);

        return result == PasswordVerificationResult.Success;
    }
}