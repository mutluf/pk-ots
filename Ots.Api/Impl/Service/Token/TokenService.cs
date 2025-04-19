using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.IdentityModel.Tokens;
using Ots.Api.Domain;
using Ots.Base;

namespace Ots.Api.Impl.Service;

public class TokenService : ITokenService
{
    private readonly JwtConfig jwtConfig;

    public TokenService(JwtConfig jwtConfig)
    {
        this.jwtConfig = jwtConfig;
    }
    public string GenerateTokenAsync(User user)
    {
        string token = GenerateToken(user);
        return token;

    }
    public string GenerateToken(User user)
    {
        var claims = GetClaims(user);
        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtConfig.Secret));
        var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

        var token = new JwtSecurityToken(
            issuer: jwtConfig.Issuer,
            audience: jwtConfig.Audience,
            claims: claims,
            expires: DateTime.Now.AddMinutes(jwtConfig.AccessTokenExpiration),
            signingCredentials: creds);

        return new JwtSecurityTokenHandler().WriteToken(token);
    }
    
    private Claim[] GetClaims(User user)
    {
        var claims = new List<Claim>
        {
            new Claim("FirstName", user.FirstName),
            new Claim("LastName", user.LastName),
            new Claim("UserName", user.UserName),
            new Claim("UserId", user.Id.ToString()),
            new Claim("Secret", user.Secret),
            new Claim(ClaimTypes.Role, user.Role)
        };

        return claims.ToArray();
    }
}
