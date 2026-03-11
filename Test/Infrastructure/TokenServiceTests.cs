using Infraestructure.Services;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using Moq;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;

namespace Test.Infrastructure
{
    public class TokenServiceTests
    {
        private const string ValidPassword = "TestSigningPasswordLongEnough123456!";

        private static TokenService CreateService(string password = ValidPassword, string lifetime = "30")
        {
            var config = new Mock<IConfiguration>();
            config.Setup(c => c["JwtSettings:serverSigningPassword"]).Returns(password);
            config.Setup(c => c["JwtSettings:tokenLifetime"]).Returns(lifetime);
            return new TokenService(config.Object);
        }

        [Fact]
        public void GenerateAccessToken_ReturnsNonEmptyString()
        {
            var service = CreateService();
            var token = service.GenerateAccessToken(Array.Empty<Claim>());
            Assert.False(string.IsNullOrEmpty(token));
        }

        [Fact]
        public void GenerateAccessToken_ReturnsParsableJwt_WithClaims()
        {
            var service = CreateService();
            var claims = new[]
            {
                new Claim(ClaimTypes.NameIdentifier, "user-1"),
                new Claim(ClaimTypes.Email, "user@test.com")
            };

            var token = service.GenerateAccessToken(claims);

            var handler = new JwtSecurityTokenHandler();
            var parsed = handler.ReadJwtToken(token);

            Assert.Equal("nexttech", parsed.Issuer);
            Assert.Contains(parsed.Claims, c => c.Value == "user-1");
            Assert.Contains(parsed.Claims, c => c.Value == "user@test.com");
        }

        [Fact]
        public void GenerateAccessToken_SetsExpiry_BasedOnLifetime()
        {
            var service = CreateService(lifetime: "60");
            var before = DateTime.UtcNow;

            var token = service.GenerateAccessToken(Array.Empty<Claim>());

            var after = DateTime.UtcNow;
            var handler = new JwtSecurityTokenHandler();
            var parsed = handler.ReadJwtToken(token);

            Assert.True(parsed.ValidTo > before.AddMinutes(59));
            Assert.True(parsed.ValidTo <= after.AddMinutes(61));
        }

        [Fact]
        public void GenerateRefreshToken_ReturnsBase64StringOf32Bytes()
        {
            var service = CreateService();
            var refresh = service.GenerateRefreshToken();

            Assert.False(string.IsNullOrEmpty(refresh));
            var bytes = Convert.FromBase64String(refresh);
            Assert.Equal(32, bytes.Length);
        }

        [Fact]
        public void GenerateRefreshToken_ReturnsDifferentValueOnEachCall()
        {
            var service = CreateService();

            var token1 = service.GenerateRefreshToken();
            var token2 = service.GenerateRefreshToken();

            Assert.NotEqual(token1, token2);
        }

        [Fact]
        public void GenerateAccessToken_UsesHmacSha256Signature()
        {
            var service = CreateService();
            var token = service.GenerateAccessToken(Array.Empty<Claim>());

            var handler = new JwtSecurityTokenHandler();
            var parsed = handler.ReadJwtToken(token);

            Assert.Equal(SecurityAlgorithms.HmacSha256, parsed.SignatureAlgorithm);
        }
    }
}
