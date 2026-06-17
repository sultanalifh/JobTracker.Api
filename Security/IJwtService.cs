using JobTracker.Api.Models;
using JobTracker.Api.Security.Models;

namespace JobTracker.Api.Security;

public interface IJwtService
{
    JwtTokenResult GenerateToken(User user);

    bool ValidateToken(string token);
}