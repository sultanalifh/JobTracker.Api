using System.Security.Cryptography;
using System.Text;
using System.Text.Json;
using JobTracker.Api.Exceptions;
using JobTracker.Api.Models;
using JobTracker.Api.Security.Models;
using JobTracker.Api.Utilities;
using Microsoft.OpenApi;

namespace JobTracker.Api.Security;

public class JwtService : IJwtService
{
    private readonly IConfiguration _configuration;

    public JwtService(IConfiguration configuration)
    {
        _configuration = configuration;
    }
    public JwtTokenResult GenerateToken(User user)
    {
        string secret = _configuration["Jwt:Secret"]!;
        int expirationMinutes = int.Parse(_configuration["Jwt:ExpirationMinutes"]!);
        DateTimeOffset exp = DateTimeOffset.UtcNow.AddMinutes(expirationMinutes);

        JwtHeader header = new JwtHeader()
        {
            Alg = "HS256",
            Typ = "JWT"
        };

        JwtPayload payload = new JwtPayload()
        {
            Sub = user.Id,
            Username = user.Username,
            Role = user.Role.GetDisplayName(),
            Exp = exp.ToUnixTimeSeconds()
        };

        string header64Url = Converter.Base64UrlEncode(Encoding.UTF8.GetBytes(JsonSerializer.Serialize(header)));
        string payload64Url = Converter.Base64UrlEncode(Encoding.UTF8.GetBytes(JsonSerializer.Serialize(payload)));

        string content = $"{header64Url}.{payload64Url}";

        string signature = Converter.Base64UrlEncode(HMACSHA256.HashData(Encoding.UTF8.GetBytes(secret), Encoding.UTF8.GetBytes(content)));

        string token = $"{header64Url}.{payload64Url}.{signature}";

        return new JwtTokenResult()
        {
            Token = token,
            ExpiresAt = exp
        };
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

        JwtPayload payloadData = JsonSerializer.Deserialize<JwtPayload>(Encoding.UTF8.GetString(Converter.Base64UrlDecode(payload)))!;

        long unixTimeNow = DateTimeOffset.UtcNow.ToUnixTimeSeconds();

        if (unixTimeNow >= payloadData.Exp)
        {
            throw new ExpiredTokenException();
        }

        string expectedSignature = Converter.Base64UrlEncode(HMACSHA256.HashData(Encoding.UTF8.GetBytes(secret), Encoding.UTF8.GetBytes(content)));

        return signature == expectedSignature;
    }
}