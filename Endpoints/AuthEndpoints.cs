using JobTracker.Api.Dtos.Request;
using JobTracker.Api.Dtos.Response;
using JobTracker.Api.Security;

namespace JobTracker.Api.Endpoints;

public static class AuthEndpoints
{
    public static void MapAuthEndpoints(this WebApplication app)
    {
        app.MapPost("/register", async (IAuthService auth, RegisterRequest request) =>
        {
            UserResponse response = await auth.Register(request);

            return response;
        });

        app.MapPost("/login", async (IAuthService auth, LoginRequest request) =>
        {
            LoginResponse response = await auth.Login(request);

            return response;
        });
    }
}