using Application.Interfaces.Services;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;

namespace Infraestructure.Services
{
    public class TokenService : ITokenService
    {
        private readonly IConfiguration _configuration;

        private string tokenLifetime;

        private string serverSigningPassword;

        public TokenService(IConfiguration configuration)
        {
            _configuration = configuration;
            tokenLifetime = _configuration["JwtSettings:tokenLifetime"] ?? string.Empty;
            serverSigningPassword = _configuration["JwtSettings:serverSigningPassword"] ?? string.Empty;
        }

        public string GenerateAccessToken(IEnumerable<Claim> claims)
        {
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(serverSigningPassword));

            var jwtToken = new JwtSecurityToken(issuer: "nexttech",
                audience: "Anyone",
                claims: claims,
                notBefore: DateTime.UtcNow,
                expires: DateTime.UtcNow.AddMinutes(int.Parse(tokenLifetime)),
                signingCredentials: new SigningCredentials(key, SecurityAlgorithms.HmacSha256)
            );

            return new JwtSecurityTokenHandler().WriteToken(jwtToken);
        }

        public string GenerateRefreshToken()
        {
            var randomNumber = new byte[32];
            using (var rng = RandomNumberGenerator.Create())
            {
                rng.GetBytes(randomNumber);
                return Convert.ToBase64String(randomNumber);
            }

        }

    }
}
