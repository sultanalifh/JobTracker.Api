using JobTracker.Api.Dtos.Request;
using JobTracker.Api.Dtos.Response;

namespace JobTracker.Api.Security;

public interface IAuthService
{
    Task<UserResponse> Register(RegisterRequest request);
    Task<LoginResponse> Login(LoginRequest request);
    Task<bool> Logout();

}