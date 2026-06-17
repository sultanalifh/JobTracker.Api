namespace JobTracker.Api.Dtos.Response;

public class UserResponse
{
    public long Id { get; set; }
    public string Username { get ; set; } = string.Empty;
    public string Role { get; set; } = string.Empty;
}