using JobTracker.Api.Models;

namespace JobTracker.Api.Security;

public interface IPasswordService
{
    string HashPassword(User user, string password);
    bool VerifyPassword(User user, string hashedPassword, string password);
}