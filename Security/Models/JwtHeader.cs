using System.Text.Json.Serialization;

namespace JobTracker.Api.Security.Models;

public class JwtHeader
{
    [JsonPropertyName("alg")]
    public string Alg { get; set; } = "HS256";
    [JsonPropertyName("typ")]
    public string Typ { get; set; } = "JWT";
}