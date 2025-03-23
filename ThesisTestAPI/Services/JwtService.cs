using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using ThesisTestAPI.Models.User;

namespace ThesisTestAPI.Services
{
    public class JwtService(IConfiguration config)
    {
        private readonly string _key = config["Jwt:Key"];
        private readonly string _issuer = config["Jwt:Issuer"];
        private readonly string _audience = config["Jwt:Audience"];
        private readonly int _expirationMinutes = int.Parse(config["Jwt:ExpirationInMinutes"]);

        public string GenerateToken(Guid? UserId)
        {
            var claims = new[]
            {
                new Claim(ClaimTypes.NameIdentifier, UserId.ToString()),
            };

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_key));
            var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var token = new JwtSecurityToken(
                _issuer,
                _audience,
                claims,
                expires: DateTime.UtcNow.AddMinutes(_expirationMinutes),
                signingCredentials: credentials
            );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }

    }
}
