using JobTracker.Api.Models;

namespace JobTracker.Api.Services;

public interface ICurrentUserService
{
    long UserId { get; }

    UserRole Role { get; }

    bool IsAuthenticated { get; }
}