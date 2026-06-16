using JobTracker.Api.Models;

namespace JobTracker.Api.Security;

public interface IJwtService
{
    string GenerateToken(User user);

    bool ValidateToken(string token);
}