namespace JobTracker.Api.Security.Models;

public class JwtTokenResult
{
    public string Token { get; set; } = string.Empty;
    public DateTimeOffset ExpiresAt { get; set; }
}