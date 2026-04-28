using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.IdentityModel.Tokens;
using RestaurantAPI.src.Modal;
using RestaurantAPI.src.Services.Interfaces;

namespace RestaurantAPI.src.Services;
/// Triển khai tạo JWT token sử dụng cấu hình từ appsettings
public class TokenService : ITokenService
{
    private readonly IConfiguration _config;

    public TokenService(IConfiguration config)
    {
        _config = config;
    }

    public string CreateToken(Account user)
    {
        var secret = _config["Jwt:Secret"] ?? throw new InvalidOperationException("JWT Secret chưa được cấu hình");
        var issuer = _config["Jwt:Issuer"] ?? "RestaurantAPI";
        var audience = _config["Jwt:Audience"] ?? "RestaurantAPI";
        var expiryMinutes = int.Parse(_config["Jwt:ExpiryMinutes"] ?? "60");

        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secret));
        var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

        var claims = new List<Claim>
        {
            new(ClaimTypes.NameIdentifier, user.Id.ToString()),
            new(ClaimTypes.Name, user.Username ?? string.Empty),
            new(JwtRegisteredClaimNames.Sub, user.Id.ToString()),
            new(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
        };

        // Thêm claim role nếu có
        claims.Add(new Claim(ClaimTypes.Role, user.Role.ToString()));

        var token = new JwtSecurityToken(
            issuer: issuer,
            audience: audience,
            claims: claims,
            expires: DateTime.UtcNow.AddMinutes(expiryMinutes),
            signingCredentials: credentials
        );

        return new JwtSecurityTokenHandler().WriteToken(token);
    }
}
