using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.IdentityModel.Tokens;

namespace identity_service.Core.Jwt;

public class JwtService
{
    private readonly IConfiguration _configuration;

    public JwtService(IConfiguration configuration)
    {
        _configuration = configuration;
    }

    public string GenerateToken(string userId, string email, string username, string role = "player")
    {
        var key = new SymmetricSecurityKey(
            Encoding.UTF8.GetBytes(_configuration["Jwt:Key"]!));
        
        var claims = new[]
        {
            new Claim(ClaimTypes.NameIdentifier, userId),
            new Claim(ClaimTypes.Email, email),
            new Claim("preferred_username", username),
            new Claim(ClaimTypes.Role, role),
            new Claim("sub", userId)
        };

        var token = new JwtSecurityToken(
            issuer: _configuration["Jwt:Issuer"],
            audience: _configuration["Jwt:Audience"],
            claims: claims,
            expires: DateTime.UtcNow.AddDays(7),
            signingCredentials: new SigningCredentials(key, SecurityAlgorithms.HmacSha256)
        );

        return new JwtSecurityTokenHandler().WriteToken(token);
    }
}