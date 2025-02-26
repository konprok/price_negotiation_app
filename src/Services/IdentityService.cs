using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.IdentityModel.Tokens;
using PriceNegotiationApp.Models;
using PriceNegotiationApp.Models.Dtos;
using PriceNegotiationApp.Services.Interfaces;

namespace PriceNegotiationApp.Services;

public class IdentityService : IIdentityService
{
    private readonly IConfiguration _configuration;

    public IdentityService(IConfiguration configuration)
    {
        _configuration = configuration;
    }

    public string GenerateToken(UserResponse user)
    {
        var claims = new List<Claim>
        {
            new (JwtRegisteredClaimNames.Jti,  Guid.NewGuid().ToString()),
            new (JwtRegisteredClaimNames.Sub,  user.Email),
            new (JwtRegisteredClaimNames.Email,user.Email),
            new ("userId", user.Id.ToString())
        };

        return CreateToken(claims);
    }

    public string GenerateToken(TokenGeneratorRequest tokenGeneratorRequest)
    {
        var claims = new List<Claim>
        {
            new (JwtRegisteredClaimNames.Jti,   Guid.NewGuid().ToString()),
            new (JwtRegisteredClaimNames.Sub,   tokenGeneratorRequest.Email),
            new (JwtRegisteredClaimNames.Email, tokenGeneratorRequest.Email),
            new ("userId", tokenGeneratorRequest.UserId.ToString())
        };

        // custom claims

        return CreateToken(claims);
    }

    private string CreateToken(IEnumerable<Claim> claims)
    {
        var tokenHandler = new JwtSecurityTokenHandler();
        
        string secretKey = _configuration["JwtSettings:Key"]
                           ?? throw new ArgumentNullException("JwtSettings:Key is missing in appsettings.json!");
        
        var keyBytes = Encoding.UTF8.GetBytes(secretKey);
        
        var tokenDescriptor = new SecurityTokenDescriptor
        {
            Subject = new ClaimsIdentity(claims),
            Expires = DateTime.UtcNow.AddHours(1),
            Issuer = _configuration["JwtSettings:Issuer"],
            Audience = _configuration["JwtSettings:Audience"],
            SigningCredentials = new SigningCredentials(
                new SymmetricSecurityKey(keyBytes),
                SecurityAlgorithms.HmacSha256
            )
        };

        var token = tokenHandler.CreateToken(tokenDescriptor);
        return tokenHandler.WriteToken(token);
    }
}