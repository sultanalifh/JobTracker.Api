using System.Text.Json.Serialization;

namespace JobTracker.Api.Security.Models;

public class JwtPayload
{
    [JsonPropertyName("sub")]
    public long Sub { get; set; }
    [JsonPropertyName("username")]
    public string Username { get; set; } = string.Empty;
    [JsonPropertyName("role")]
    public string Role { get; set; } = string.Empty;
    [JsonPropertyName("exp")]
    public long Exp { get; set; }
}