namespace JobTracker.Api.Dtos.Response;

public class LoginResponse
{
    public string Token { get; set; } = string.Empty;
    public DateTimeOffset ExpiresAt { get; set; }
    public UserResponse User { get; set; }
}