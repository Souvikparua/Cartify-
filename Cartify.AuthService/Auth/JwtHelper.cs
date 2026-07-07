using System.Security.Cryptography;
using System.Text;
using Microsoft.IdentityModel.JsonWebTokens;
using Microsoft.IdentityModel.Tokens;
using Cartify.AuthService.Entities;

namespace Cartify.AuthService.Auth;

// Central JWT settings + token issuance. In production, load the key from a secret store / env var.
public static class JwtHelper
{
    public const string Key = "Cartify-Shared-JWT-Signing-Secret-Key-Min-32-Bytes-Long-2026";
    public const string Issuer = "Cartify";
    public const string Audience = "Cartify";

    public static SymmetricSecurityKey SigningKey() =>
        new(Encoding.UTF8.GetBytes(Key));

    public static string GenerateToken(User user)
    {
        var descriptor = new SecurityTokenDescriptor
        {
            Issuer = Issuer,
            Audience = Audience,
            Expires = DateTime.UtcNow.AddHours(8),
            SigningCredentials = new SigningCredentials(SigningKey(), SecurityAlgorithms.HmacSha256),
            Claims = new Dictionary<string, object>
            {
                ["sub"] = user.Email,
                ["email"] = user.Email,
                ["name"] = user.FullName,
                ["role"] = user.Role
            }
        };
        return new JsonWebTokenHandler().CreateToken(descriptor);
    }

    public static string GenerateOtp() =>
        RandomNumberGenerator.GetInt32(100000, 1000000).ToString();
}
