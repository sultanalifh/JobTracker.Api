using System.Security.Cryptography;
using System.Text;
using System.Text.Json;
using JobTracker.Api.Exceptions;
using JobTracker.Api.Models;
using JobTracker.Api.Utilities;

namespace JobTracker.Api.Security;

public class JwtService : IJwtService
{
    private readonly IConfiguration _configuration;

    public JwtService(IConfiguration configuration)
    {
        _configuration = configuration;
    }
    public string GenerateToken(User user)
    {
        string secret = _configuration["Jwt:Secret"]!;
        int expirationMinutes = int.Parse(_configuration["Jwt:ExpirationMinutes"]!);
        long exp = DateTimeOffset.UtcNow.AddMinutes(expirationMinutes).ToUnixTimeSeconds();

        var header = new
        {
            alg = "HS256",
            typ = "JWT"
        };

        var payload = new
        {
            Sub = user.Id,
            Username = user.Username,
            Role = user.Role,
            Exp = exp
        };

        string header64Url = Converter.Base64UrlEncode(Encoding.UTF8.GetBytes(JsonSerializer.Serialize(header)));
        string payload64Url = Converter.Base64UrlEncode(Encoding.UTF8.GetBytes(JsonSerializer.Serialize(payload)));

        string content = $"{header64Url}.{payload64Url}";

        string signature = Converter.Base64UrlEncode(HMACSHA256.HashData(Encoding.UTF8.GetBytes(secret), Encoding.UTF8.GetBytes(content)));

        return $"{header64Url}.{payload64Url}.{signature}";
    }

    public bool ValidateToken(string token)
    {
        string secret = _configuration["Jwt:Secret"]!;

        string[] parts = token.Split('.');

        if(parts.Length != 3)
        {
            throw new MalformedTokenException();
        }

        string header = parts[0];
        string payload = parts[1];
        string signature = parts[2];

        string content = $"{header}.{payload}";

        string expectedSignature = Converter.Base64UrlEncode(HMACSHA256.HashData(Encoding.UTF8.GetBytes(secret), Encoding.UTF8.GetBytes(content)));

        return signature == expectedSignature;
    }
}